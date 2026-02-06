using backend.Core.DTOs.Auth;
using backend.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.API.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService, 
        IUserRepository userRepository,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and get JWT tokens
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            _logger.LogWarning("Login failed for user: {Username}", request.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }

        _logger.LogInformation("Login successful for user: {Username}", request.Username);
        return Ok(result);
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for username: {Username}", request.Username);

        var result = await _authService.RegisterAsync(request);

        if (result == null)
        {
            _logger.LogWarning("Registration failed for username: {Username}", request.Username);
            return BadRequest(new { message = "Username already exists or invalid role" });
        }

        _logger.LogInformation("Registration successful for username: {Username}", request.Username);
        return CreatedAtAction(nameof(Login), result);
    }

    /// <summary>
    /// Refresh JWT tokens using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Logout user and invalidate refresh token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _authService.LogoutAsync(userId);

        if (!result)
        {
            return BadRequest(new { message = "Logout failed" });
        }

        return Ok(new { message = "Logout successful" });
    }

    /// <summary>
    /// Get current authenticated user info
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var firstname = User.FindFirst("firstname")?.Value;
        var lastname = User.FindFirst("lastname")?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            userId,
            username,
            email,
            firstname,
            lastname,
            roles
        });
    }

    /// <summary>
    /// Setup endpoint - Create first admin user (only works when no users exist)
    /// </summary>
    [HttpPost("setup")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Setup([FromBody] RegisterRequest request)
    {
        // Check if any users already exist
        var existingUsers = await _userRepository.GetAllAsync();
        if (existingUsers.Any())
        {
            _logger.LogWarning("Setup attempted but users already exist");
            return BadRequest(new { message = "Setup already completed. Use /register with admin credentials." });
        }

        _logger.LogInformation("Initial setup - creating first admin user: {Username}", request.Username);

        // Force the role to Admin for the first user
        var adminRequest = request with { RoleId = "1" }; // RoleId "1" is Admin

        var result = await _authService.RegisterAsync(adminRequest);

        if (result == null)
        {
            _logger.LogWarning("Setup failed for username: {Username}", request.Username);
            return BadRequest(new { message = "Setup failed - check request data" });
        }

        _logger.LogInformation("Initial admin setup successful for username: {Username}", request.Username);
        return CreatedAtAction(nameof(Login), result);
    }
}
