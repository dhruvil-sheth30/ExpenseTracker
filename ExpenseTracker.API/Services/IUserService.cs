using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Services
{
    public interface IUserService
    {
        Task<bool> UserExistsAsync(string username);
        Task<bool> CreateUserAsync(string username, string password);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByIdAsync(int id);
        bool VerifyPassword(string password, string passwordHash);
    }
}
