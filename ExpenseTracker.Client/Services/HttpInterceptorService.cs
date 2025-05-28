using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenseTracker.Client.Services
{
    public class HttpInterceptorService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public HttpInterceptorService(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task InterceptBeforeHttpRequest()
        {
            try
            {
                var token = await _localStorage.GetItemAsStringAsync("authToken");
                
                if (string.IsNullOrEmpty(token))
                    return;
                
                // Validate token before using it
                var tokenHandler = new JwtSecurityTokenHandler();
                if (!tokenHandler.CanReadToken(token))
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    _navigationManager.NavigateTo("/login");
                    return;
                }
                
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            catch
            {
                // If there's an error processing the token, clear it and redirect
                await _localStorage.RemoveItemAsync("authToken");
                _navigationManager.NavigateTo("/login");
            }
        }

        public void InterceptAfterHttpResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("/login");
            }
        }
    }
}
