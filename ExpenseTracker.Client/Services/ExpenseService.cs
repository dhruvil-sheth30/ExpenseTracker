using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using ExpenseTracker.Shared.Models;

namespace ExpenseTracker.Client.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly string _baseUrl = "api/expenses";

        public ExpenseService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<IEnumerable<Expense>> GetExpensesAsync(DateTime? startDate = null, DateTime? endDate = null, string? category = null)
        {
            try
            {
                var url = $"{_baseUrl}";
                
                if (startDate.HasValue)
                    url += url.Contains("?") ? $"&startDate={startDate.Value:yyyy-MM-dd}" : $"?startDate={startDate.Value:yyyy-MM-dd}";
                
                if (endDate.HasValue)
                    url += url.Contains("?") ? $"&endDate={endDate.Value:yyyy-MM-dd}" : $"?endDate={endDate.Value:yyyy-MM-dd}";
                
                if (!string.IsNullOrEmpty(category))
                    url += url.Contains("?") ? $"&category={category}" : $"?category={category}";

                var result = await _httpClient.GetFromJsonAsync<IEnumerable<Expense>>(url);
                return result ?? Array.Empty<Expense>();
            }
            catch
            {
                return Array.Empty<Expense>();
            }
        }

        public async Task<Expense?> GetExpenseByIdAsync(int userId, int expenseId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Expense>($"{_baseUrl}/{expenseId}?userId={userId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<Expense> AddExpenseAsync(Expense expense)
        {
            // Get user ID from local storage and set it on the expense
            var userId = await _localStorage.GetItemAsync<int>("userId");
            expense.UserId = userId;

            var response = await _httpClient.PostAsJsonAsync(_baseUrl, expense);
            
            // If unauthorized, throw an exception
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to create expenses. Please log in again.");
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Expense>();
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{expense.Id}", expense);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteExpenseAsync(int expenseId)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{expenseId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<ExpenseSummary> GetExpenseSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = $"{_baseUrl}/summary";
                
                if (startDate.HasValue)
                    url += url.Contains("?") ? $"&startDate={startDate.Value:yyyy-MM-dd}" : $"?startDate={startDate.Value:yyyy-MM-dd}";
                
                if (endDate.HasValue)
                    url += url.Contains("?") ? $"&endDate={endDate.Value:yyyy-MM-dd}" : $"?endDate={endDate.Value:yyyy-MM-dd}";

                var result = await _httpClient.GetFromJsonAsync<ExpenseSummary>(url);
                return result ?? new ExpenseSummary();
            }
            catch
            {
                return new ExpenseSummary();
            }
        }
    }
}
