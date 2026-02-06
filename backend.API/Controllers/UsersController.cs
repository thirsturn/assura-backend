using backend.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

/// <summary>
/// Controller for user management (demonstrates role-based access)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();

        var result = users.Select(u => new
        {
            u.userID,
            u.firstname,
            u.lastname,
            u.email,
            u.username,
            u.isBlocked,
            u.lastlogin,
            u.createdOn,
            division = u.Division?.divisionName,
            roles = u.UserRoles.Select(ur => ur.Role.roleName).ToList()
        });

        return Ok(result);
    }

    /// <summary>
    /// Get user by ID (Admin or StoreKeeper)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "StoreKeeperOrAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new
        {
            user.userID,
            user.firstname,
            user.lastname,
            user.email,
            user.username,
            user.telephone,
            user.isBlocked,
            user.lastlogin,
            user.createdOn,
            division = user.Division?.divisionName,
            divisionId = user.divisionId,
            roles = user.UserRoles.Select(ur => ur.Role.roleName).ToList()
        });
    }

    /// <summary>
    /// Block/Unblock a user (Admin only)
    /// </summary>
    [HttpPatch("{id}/block")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleBlockUser(string id, [FromBody] BlockUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.isBlocked = request.IsBlocked;
        user.lastUpdated = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("User {UserId} blocked status changed to: {IsBlocked}", id, request.IsBlocked);

        return Ok(new { message = request.IsBlocked ? "User blocked" : "User unblocked" });
    }

    /// <summary>
    /// Delete a user (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var exists = await _userRepository.ExistsAsync(id);

        if (!exists)
        {
            return NotFound(new { message = "User not found" });
        }

        await _userRepository.DeleteAsync(id);
        _logger.LogInformation("User {UserId} deleted", id);

        return Ok(new { message = "User deleted successfully" });
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    [HttpGet("roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleRepository.GetAllAsync();
        return Ok(roles.Select(r => new { r.roleID, r.roleName }));
    }
}

/// <summary>
/// Request model for blocking/unblocking users
/// </summary>
public record BlockUserRequest(bool IsBlocked);
