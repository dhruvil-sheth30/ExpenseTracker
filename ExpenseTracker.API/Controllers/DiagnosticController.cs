using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DiagnosticController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("env-check")]
        [Authorize(Roles = "Admin")] // Restrict access to admins only
        public IActionResult CheckEnvironmentVariables()
        {
            var envVars = new Dictionary<string, string>
            {
                { "JWT_SECRET", Environment.GetEnvironmentVariable("JWT_SECRET") ?? "Not set" },
                { "JWT_ISSUER", Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "Not set" },
                { "JWT_AUDIENCE", Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "Not set" },
                { "CONNECTION_STRING", "Hidden for security" } // Don't expose connection string
            };

            var configValues = new Dictionary<string, string>
            {
                { "JwtSettings:Key", "Hidden for security" },
                { "JwtSettings:Issuer", _configuration["JwtSettings:Issuer"] ?? "Not set" },
                { "JwtSettings:Audience", _configuration["JwtSettings:Audience"] ?? "Not set" }
            };

            return Ok(new
            {
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                EnvironmentVariables = envVars,
                ConfigurationValues = configValues
            });
        }
    }
}
