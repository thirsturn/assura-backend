using System.Security.Claims;

namespace backend.Core.Interfaces;

/// <summary>
/// Service interface for JWT token operations
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    DateTime GetAccessTokenExpiration();
}
