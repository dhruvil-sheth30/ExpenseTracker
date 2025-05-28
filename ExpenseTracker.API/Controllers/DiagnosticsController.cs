using ExpenseTracker.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public DiagnosticsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("db-connection")]
        public async Task<IActionResult> CheckDatabaseConnection()
        {
            try
            {
                // Try to connect to the database
                bool canConnect = await _context.Database.CanConnectAsync();

                // Return the connection string (masked) for debugging
                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Not configured";
                string maskedConnectionString = MaskConnectionString(connectionString);

                return Ok(new
                {
                    Success = canConnect,
                    Message = canConnect ? "Successfully connected to the database" : "Failed to connect to the database",
                    ConnectionString = maskedConnectionString
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error checking database connection: {ex.Message}",
                    Error = ex.ToString()
                });
            }
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")] // Remove this line for testing, but add it back for security
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new { u.Id, u.Username })
                    .ToListAsync();
                    
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error retrieving users: {ex.Message}"
                });
            }
        }

        private string MaskConnectionString(string connectionString)
        {
            // Simple masking to hide sensitive information
            if (string.IsNullOrEmpty(connectionString)) return connectionString;

            var parts = connectionString.Split(';');
            var masked = new List<string>();

            foreach (var part in parts)
            {
                if (part.ToLower().Contains("password"))
                {
                    // Mask the password
                    var keyValue = part.Split('=');
                    if (keyValue.Length == 2)
                    {
                        masked.Add($"{keyValue[0]}=********");
                    }
                    else
                    {
                        masked.Add(part);
                    }
                }
                else
                {
                    masked.Add(part);
                }
            }

            return string.Join(";", masked);
        }
    }
}
