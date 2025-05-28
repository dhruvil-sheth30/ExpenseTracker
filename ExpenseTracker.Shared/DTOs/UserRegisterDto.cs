using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Shared.DTOs
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "The Username field is required.")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "The Password field is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
        
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
