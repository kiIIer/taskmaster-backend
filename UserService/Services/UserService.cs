using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Services;

public class UserService(AppDbContext dbContext) : IUserService
{
    private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<User> CreateUserAsync(UserCreate userCreate)
    {
        ArgumentNullException.ThrowIfNull(userCreate);

        // Check if email already exists
        if (await _dbContext.Users.AnyAsync(u => u.Email == userCreate.Email))
            throw new InvalidOperationException("Email already exists");

        var user = new User
        {
            Email = userCreate.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreate.Password),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> AuthenticateUserAsync(UserLogin userLogin)
    {
        if (userLogin == null)
            throw new ArgumentNullException(nameof(userLogin));

        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == userLogin.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(userLogin.Password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }
}