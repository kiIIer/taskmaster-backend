using UserService.Models;

namespace UserService.Interfaces;

public interface IUserService
{
    Task<User> CreateUserAsync(UserCreate userCreate);
    Task<User> AuthenticateUserAsync(UserLogin userLogin);
    Task<User> GetUserByIdAsync(int id);
}