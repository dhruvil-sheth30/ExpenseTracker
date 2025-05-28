using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
