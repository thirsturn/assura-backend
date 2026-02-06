using System.Security.Claims;
using backend.Core;
using backend.Core.DTOs.Auth;
using backend.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace backend.Infrastructure.Services;

/// <summary>
/// Service for authentication operations
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly int _refreshTokenExpirationDays;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _configuration = configuration;
        _refreshTokenExpirationDays = int.Parse(
            _configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
    }

    public async Task<TokenResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        
        if (user == null)
            return null;

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.passwordHash))
            return null;

        // Check if user is blocked
        if (user.isBlocked)
            return null;

        // Get user roles
        var roles = user.UserRoles.Select(ur => ur.Role.roleName).ToList();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Update user with refresh token
        user.refreshToken = refreshToken;
        user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
        user.lastlogin = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return new TokenResponse(
            accessToken,
            refreshToken,
            _tokenService.GetAccessTokenExpiration(),
            user.userID,
            user.username,
            roles
        );
    }

    public async Task<TokenResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check if username already exists
        if (await _userRepository.UsernameExistsAsync(request.Username))
            return null;

        // Verify role exists
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role == null)
            return null;

        // Create new user
        var user = new User
        {
            userID = Guid.NewGuid().ToString(),
            firstname = request.Firstname,
            lastname = request.Lastname,
            email = request.Email,
            telephone = request.Telephone,
            username = request.Username,
            passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            divisionId = request.DivisionId,
            createdOn = DateTime.UtcNow,
            lastUpdated = DateTime.UtcNow,
            lastlogin = DateTime.UtcNow,
            isBlocked = false
        };

        // Save user
        await _userRepository.CreateAsync(user);

        // Add user role
        user.UserRoles.Add(new UserRole
        {
            UserId = user.userID,
            RoleId = request.RoleId
        });
        await _userRepository.UpdateAsync(user);

        // Load user with roles
        user = await _userRepository.GetByIdAsync(user.userID);
        var roles = user!.UserRoles.Select(ur => ur.Role.roleName).ToList();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Update user with refresh token
        user.refreshToken = refreshToken;
        user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
        await _userRepository.UpdateAsync(user);

        return new TokenResponse(
            accessToken,
            refreshToken,
            _tokenService.GetAccessTokenExpiration(),
            user.userID,
            user.username,
            roles
        );
    }

    public async Task<TokenResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // Get principal from expired token
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return null;

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return null;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || 
            user.refreshToken != request.RefreshToken ||
            user.refreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        // Get user roles
        var roles = user.UserRoles.Select(ur => ur.Role.roleName).ToList();

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Update user with new refresh token
        user.refreshToken = newRefreshToken;
        user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
        await _userRepository.UpdateAsync(user);

        return new TokenResponse(
            newAccessToken,
            newRefreshToken,
            _tokenService.GetAccessTokenExpiration(),
            user.userID,
            user.username,
            roles
        );
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        // Invalidate refresh token
        user.refreshToken = null;
        user.refreshTokenExpiryTime = null;
        await _userRepository.UpdateAsync(user);

        return true;
    }
}
