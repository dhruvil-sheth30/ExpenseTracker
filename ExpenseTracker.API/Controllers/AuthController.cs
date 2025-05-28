using ExpenseTracker.API.Services;
using ExpenseTracker.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Invalid registration data" 
                });
            }

            // Check if username already exists
            if (await _userService.UserExistsAsync(registerDto.Username))
            {
                return BadRequest(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Username already exists" 
                });
            }

            // Create user
            var result = await _userService.CreateUserAsync(registerDto.Username, registerDto.Password);
            if (!result)
            {
                return BadRequest(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "User registration failed" 
                });
            }

            // Generate token
            var user = await _userService.GetUserByUsernameAsync(registerDto.Username);
            var token = _tokenService.GenerateToken(user!);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Username = user!.Username,
                UserId = user.Id,
                Token = token,
                Message = "Registration successful"
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Invalid login data" 
                });
            }

            // Verify credentials
            var user = await _userService.GetUserByUsernameAsync(loginDto.Username);
            if (user == null || !_userService.VerifyPassword(loginDto.Password ?? string.Empty, user.PasswordHash))
            {
                return Unauthorized(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Invalid username or password" 
                });
            }

            // Generate token
            var token = _tokenService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Username = user.Username,
                UserId = user.Id,
                Token = token,
                Message = "Login successful"
            });
        }
    }
}
