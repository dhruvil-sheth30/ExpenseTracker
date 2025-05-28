using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Shared.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string? Category { get; set; }

        public int UserId { get; set; }
    }
}
