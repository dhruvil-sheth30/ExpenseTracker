using ExpenseTracker.Shared.Models;

namespace ExpenseTracker.API.Services
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetExpensesAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, string? category = null);
        Task<Expense?> GetExpenseByIdAsync(int id, int userId);
        Task<Expense> AddExpenseAsync(Expense expense);
        Task<bool> UpdateExpenseAsync(Expense expense);
        Task<bool> DeleteExpenseAsync(int id);
        Task<bool> ExpenseExistsAsync(int id, int userId);
        Task<object> GetExpenseSummaryAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
