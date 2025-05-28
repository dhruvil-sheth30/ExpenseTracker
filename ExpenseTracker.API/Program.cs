using ExpenseTracker.API.Data;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using DotNetEnv;

// Explicitly load .env file from the application root directory
var rootPath = Directory.GetCurrentDirectory();
var envPath = Path.Combine(rootPath, ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine("Loaded environment variables from .env file");
}
else
{
    Console.WriteLine(".env file not found at: " + envPath);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expense Tracker API", Version = "v1" });
});

// Update the JWT Configuration section to use environment variables
// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? jwtSettings["Key"] ?? 
    throw new InvalidOperationException("JWT Key is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings["Issuer"],
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Replace hardcoded connection string with env var
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? 
    builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string not found in environment or configuration.");

// Update the database context configuration with retry logic
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// Register application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? 
                new[] { 
                    "http://localhost:5173", 
                    "https://localhost:7001",
                    "https://expensetracker-client.vercel.app", // Your deployed frontend URL
                    "https://your-frontend-domain.com" 
                })
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Build the app
var app = builder.Build();

// Update the database initialization with error handling
// Ensure the database is created and up to date
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Attempting to ensure database is created...");
        // Use a timeout for the database creation attempt
        var task = Task.Run(() => dbContext.Database.EnsureCreated());
        if (task.Wait(TimeSpan.FromSeconds(60)))
        {
            logger.LogInformation("Database check completed successfully.");
        }
        else
        {
            logger.LogWarning("Database creation check timed out, but the application will continue.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while ensuring the database exists. The application will continue, but some features might not work correctly.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Database connectivity verification endpoint
app.MapGet("/api/dbconnection", async (AppDbContext dbContext) =>
{
    try
    {
        bool canConnect = await dbContext.Database.CanConnectAsync();
        return Results.Ok(new { Connected = canConnect, Message = canConnect ? "Successfully connected to the database" : "Failed to connect to the database" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Connected = false, Message = $"Error: {ex.Message}" });
    }
});

app.Run();
