using System.Threading.Tasks;
using ExpenseTracker.Shared.DTOs;

namespace ExpenseTracker.Client.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(UserLoginDto loginDto);
        Task<AuthResponseDto> Register(UserRegisterDto registerDto);
        Task Logout();
        Task<bool> IsUserAuthenticated();
        
        // Add these methods to match what's being called in the components
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(UserRegisterDto registerDto);
        Task LogoutAsync();
    }
}
