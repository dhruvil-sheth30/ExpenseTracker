using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ExpenseTracker.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenseTracker.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private const string AuthTokenKey = "authToken";
        private const string UserIdKey = "userId";

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<AuthResponseDto> Login(UserLoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
                
                if (!response.IsSuccessStatusCode)
                {
                    return new AuthResponseDto { 
                        Success = false, 
                        Message = $"Login failed with status code: {response.StatusCode}" 
                    };
                }
                
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                if (result != null && result.Success && !string.IsNullOrEmpty(result.Token))
                {
                    // Validate token format before storing
                    var tokenHandler = new JwtSecurityTokenHandler();
                    if (!tokenHandler.CanReadToken(result.Token))
                    {
                        return new AuthResponseDto { 
                            Success = false, 
                            Message = "Invalid token format received from server" 
                        };
                    }

                    await _localStorage.SetItemAsStringAsync(AuthTokenKey, result.Token);
                    await _localStorage.SetItemAsync(UserIdKey, result.UserId);
                    ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
                    return result;
                }
                
                return new AuthResponseDto { 
                    Success = false, 
                    Message = result?.Message ?? "Login failed. Invalid or missing token." 
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto { 
                    Success = false, 
                    Message = $"Login exception: {ex.Message}" 
                };
            }
        }

        public async Task<AuthResponseDto> Register(UserRegisterDto registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
            return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(AuthTokenKey);
            await _localStorage.RemoveItemAsync(UserIdKey);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public async Task<bool> IsUserAuthenticated()
        {
            try
            {
                var token = await _localStorage.GetItemAsStringAsync(AuthTokenKey);
                if (string.IsNullOrEmpty(token))
                    return false;
                
                // Check if token is valid format
                var tokenHandler = new JwtSecurityTokenHandler();
                if (!tokenHandler.CanReadToken(token))
                {
                    await _localStorage.RemoveItemAsync(AuthTokenKey);
                    return false;
                }
                
                var jwt = tokenHandler.ReadJwtToken(token);
                var expiry = jwt.ValidTo;
                
                // Check if token is expired
                if (expiry < DateTime.UtcNow)
                {
                    await _localStorage.RemoveItemAsync(AuthTokenKey);
                    return false;
                }
                
                return true;
            }
            catch
            {
                await _localStorage.RemoveItemAsync(AuthTokenKey);
                return false;
            }
        }

        // Implement the missing methods by simply calling the existing ones
        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            return await Login(loginDto);
        }

        public async Task<AuthResponseDto> RegisterAsync(UserRegisterDto registerDto)
        {
            return await Register(registerDto);
        }

        public async Task LogoutAsync()
        {
            await Logout();
        }
    }
}
