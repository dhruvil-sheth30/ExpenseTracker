using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Shared.Models;

namespace ExpenseTracker.Client.Services
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetExpensesAsync(DateTime? startDate = null, DateTime? endDate = null, string? category = null);
        Task<Expense?> GetExpenseByIdAsync(int userId, int expenseId);
        Task<Expense> AddExpenseAsync(Expense expense);
        Task UpdateExpenseAsync(Expense expense);
        Task DeleteExpenseAsync(int id);
        Task<ExpenseSummary> GetExpenseSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
