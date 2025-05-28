using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

namespace ExpenseTracker.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsStringAsync("authToken");

                if (string.IsNullOrWhiteSpace(token))
                    return CreateAnonymousState();

                // Check if the token is valid JWT format
                if (!_tokenHandler.CanReadToken(token))
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    return CreateAnonymousState();
                }

                var tokenContent = _tokenHandler.ReadJwtToken(token);
                var expiry = tokenContent.ValidTo;

                if (expiry < DateTime.UtcNow)
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    return CreateAnonymousState();
                }

                var claims = tokenContent.Claims;
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                
                return new AuthenticationState(user);
            }
            catch (Exception)
            {
                // If there's any error reading the token, clear it and return anonymous state
                await _localStorage.RemoveItemAsync("authToken");
                return CreateAnonymousState();
            }
        }

        private AuthenticationState CreateAnonymousState()
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public void NotifyUserAuthentication(string token)
        {
            if (string.IsNullOrEmpty(token) || !_tokenHandler.CanReadToken(token))
            {
                NotifyUserLogout();
                return;
            }

            try
            {
                var tokenContent = _tokenHandler.ReadJwtToken(token);
                var claims = tokenContent.Claims;
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                var authState = Task.FromResult(new AuthenticationState(user));
                NotifyAuthenticationStateChanged(authState);
            }
            catch
            {
                NotifyUserLogout();
            }
        }

        public void NotifyUserLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
