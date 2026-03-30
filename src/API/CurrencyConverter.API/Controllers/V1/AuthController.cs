using Asp.Versioning;
using CurrencyConverter.BusinessLogic.DTOs.Auth;
using CurrencyConverter.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.API.Controllers.V1
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController(ITokenService tokenService, ILogger<AuthController> logger) : ControllerBase
    {
        private static readonly Dictionary<string, (string PasswordHash, string Role)> Users = new()
        {
            { "admin", ("admin123", "Admin") },
            { "user",  ("user123",  "User")  }
        };

        /// <summary>
        /// Authenticate and receive a JWT token.
        /// Demo credentials — Admin: admin/admin123 | User: user/user123
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!Users.TryGetValue(request.Username, out var userData) ||
                userData.PasswordHash != request.Password)
            {
                logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                return Unauthorized(new { error = "Invalid credentials." });
            }

            var token = tokenService.GenerateToken(request.Username, userData.Role);

            return Ok(token);
        }
    }
}
