using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.DTOs;
using StudentManagement.Core.Interfaces;

namespace StudentManagement.API.Controllers;

/// <summary>
/// Authentication - Generate JWT Token
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IJwtService jwtService, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login to get a JWT token. Use admin/Admin@123 or user/User@123
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.FailResponse("Validation failed", errors));
        }

        // Demo credentials - in production, validate against DB with hashed passwords
        var users = new Dictionary<string, (string Password, string Role)>
        {
            { "admin", ("Admin@123", "Admin") },
            { "user", ("User@123", "User") }
        };

        if (!users.TryGetValue(request.Username.ToLower(), out var userInfo) || userInfo.Password != request.Password)
        {
            _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
            return Unauthorized(ApiResponse<LoginResponseDto>.FailResponse("Invalid username or password"));
        }

        var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60");
        var token = _jwtService.GenerateToken(request.Username, userInfo.Role);

        var response = new LoginResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Username = request.Username
        };

        _logger.LogInformation("User {Username} logged in successfully", request.Username);
        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful"));
    }
}
