namespace backend.Core.DTOs.Auth;

/// <summary>
/// Response model containing JWT tokens
/// </summary>
public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime Expiration,
    string UserId,
    string Username,
    IEnumerable<string> Roles
);
