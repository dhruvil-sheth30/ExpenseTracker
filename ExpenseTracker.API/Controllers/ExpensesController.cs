using System.Security.Claims;
using ExpenseTracker.API.Services;
using ExpenseTracker.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetAllExpenses([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? category)
        {
            var userId = GetUserId();
            var expenses = await _expenseService.GetExpensesAsync(userId, startDate, endDate, category);
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var userId = GetUserId();
            var expense = await _expenseService.GetExpenseByIdAsync(id, userId);

            if (expense == null)
            {
                return NotFound();
            }

            return Ok(expense);
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> AddExpense(Expense expense)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            expense.UserId = GetUserId();
            var createdExpense = await _expenseService.AddExpenseAsync(expense);
            return CreatedAtAction(nameof(GetExpense), new { id = createdExpense.Id }, createdExpense);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, Expense expense)
        {
            if (id != expense.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            expense.UserId = userId;

            var exists = await _expenseService.ExpenseExistsAsync(id, userId);
            if (!exists)
            {
                return NotFound();
            }

            await _expenseService.UpdateExpenseAsync(expense);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var userId = GetUserId();
            var exists = await _expenseService.ExpenseExistsAsync(id, userId);
            if (!exists)
            {
                return NotFound();
            }

            await _expenseService.DeleteExpenseAsync(id);
            return NoContent();
        }

        [HttpGet("summary")]
        public async Task<ActionResult> GetExpenseSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var userId = GetUserId();
            var summary = await _expenseService.GetExpenseSummaryAsync(userId, startDate, endDate);
            return Ok(summary);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        }
    }
}
