using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Shared.DTOs
{
    public class UserLoginDto
    {
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Username { get; set; }
    }
}
