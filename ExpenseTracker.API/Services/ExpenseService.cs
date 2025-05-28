using ExpenseTracker.API.Data;
using ExpenseTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetExpensesAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, string? category = null)
        {
            var query = _context.Expenses.Where(e => e.UserId == userId);

            if (startDate.HasValue)
            {
                query = query.Where(e => e.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.Date <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category == category);
            }

            var apiExpenses = await query.OrderByDescending(e => e.Date).ToListAsync();
            // Map API model to Shared model
            return apiExpenses.Select(e => new ExpenseTracker.Shared.Models.Expense
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Amount = e.Amount,
                Date = e.Date,
                Category = e.Category,
                UserId = e.UserId
            });
        }

        public async Task<Expense?> GetExpenseByIdAsync(int id, int userId)
        {
            var e = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (e == null) return null;
            return new ExpenseTracker.Shared.Models.Expense
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Amount = e.Amount,
                Date = e.Date,
                Category = e.Category,
                UserId = e.UserId
            };
        }

        public async Task<Expense> AddExpenseAsync(Expense expense)
        {
            // Map Shared model to API model
            var apiExpense = new ExpenseTracker.Shared.Models.Expense
            {
                Title = expense.Title ?? "",
                Description = expense.Description,
                Amount = expense.Amount,
                Date = expense.Date,
                Category = expense.Category ?? "",
                UserId = expense.UserId
            };
            _context.Expenses.Add(apiExpense);
            await _context.SaveChangesAsync();
            // Map back to Shared model
            return new ExpenseTracker.Shared.Models.Expense
            {
                Id = apiExpense.Id,
                Title = apiExpense.Title,
                Description = apiExpense.Description,
                Amount = apiExpense.Amount,
                Date = apiExpense.Date,
                Category = apiExpense.Category,
                UserId = apiExpense.UserId
            };
        }

        public async Task<bool> UpdateExpenseAsync(Expense expense)
        {
            var apiExpense = await _context.Expenses.FindAsync(expense.Id);
            if (apiExpense == null) return false;
            apiExpense.Title = expense.Title ?? "";
            apiExpense.Description = expense.Description;
            apiExpense.Amount = expense.Amount;
            apiExpense.Date = expense.Date;
            apiExpense.Category = expense.Category ?? "";
            apiExpense.UserId = expense.UserId;
            _context.Entry(apiExpense).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return false;
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExpenseExistsAsync(int id, int userId)
        {
            return await _context.Expenses.AnyAsync(e => e.Id == id && e.UserId == userId);
        }

        public async Task<object> GetExpenseSummaryAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Expenses.Where(e => e.UserId == userId);

            if (startDate.HasValue)
            {
                query = query.Where(e => e.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.Date <= endDate.Value);
            }

            var expenses = await query.ToListAsync();

            var totalAmount = expenses.Sum(e => e.Amount);

            var categoryTotals = expenses
                .GroupBy(e => e.Category)
                .Select(g => new CategoryTotal { Category = g.Key, Amount = g.Sum(e => e.Amount) })
                .OrderByDescending(x => x.Amount)
                .ToList();

            var monthlyTotals = expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new MonthlyTotal
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Amount = g.Sum(e => e.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return new ExpenseSummary
            {
                TotalAmount = totalAmount,
                CategoryTotals = categoryTotals,
                MonthlyTotals = monthlyTotals,
                TotalCount = expenses.Count
            };
        }
    }
}
