using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Services;

public class UserService(AppDbContext dbContext, IConfiguration configuration) : IUserService
{
    private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private readonly string _jwtSecret = configuration["Jwt:Secret"] ??
                                         throw new ArgumentNullException("Jwt:Secret is missing in configuration");

    public async Task<User> CreateUserAsync(UserCreate userCreate)
    {
        if (userCreate == null)
            throw new ArgumentNullException(nameof(userCreate));

        if (await _dbContext.Users.AnyAsync(u => u.Email == userCreate.Email))
            throw new InvalidOperationException("Email already exists");

        var user = new User
        {
            Email = userCreate.Email,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<string> GenerateJwtTokenAsync(UserLogin userLogin)
    {
        if (userLogin == null)
            throw new ArgumentNullException(nameof(userLogin));

        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == userLogin.Email);
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Generate JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }
}