using System;
using System.Collections.Generic;

namespace ExpenseTracker.Shared.Models
{
    public class ExpenseSummary
    {
        public decimal TotalAmount { get; set; }
        public List<CategoryTotal> CategoryTotals { get; set; } = new List<CategoryTotal>();
        public List<MonthlyTotal> MonthlyTotals { get; set; } = new List<MonthlyTotal>();
        public int TotalCount { get; set; }
    }

    public class CategoryTotal
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class MonthlyTotal
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
