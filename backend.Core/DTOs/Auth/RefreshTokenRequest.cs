namespace backend.Core.DTOs.Auth;

/// <summary>
/// Request model for refreshing tokens
/// </summary>
public record RefreshTokenRequest(string AccessToken, string RefreshToken);
