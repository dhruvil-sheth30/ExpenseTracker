using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace ExpenseTracker.Client.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHeaderHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization == null)
            {
                var token = await _localStorage.GetItemAsStringAsync("authToken");
                
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine($"Added Authorization header: Bearer {token.Substring(0, Math.Min(token.Length, 20))}..."); 
                }
                else
                {
                    Console.WriteLine("No token found in localStorage");
                }
            }

            var response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine($"Response status: {response.StatusCode} for URL {request.RequestUri}");
            
            return response;
        }
    }
}
