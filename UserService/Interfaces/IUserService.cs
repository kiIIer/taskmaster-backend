using UserService.Models;

namespace UserService.Interfaces;

public interface IUserService
{
    Task<User> CreateUserAsync(UserCreate userCreate);
    Task<string> GenerateJwtTokenAsync(UserLogin userLogin); // Returns a JWT instead of a User
    Task<User> GetUserByIdAsync(int id);
}