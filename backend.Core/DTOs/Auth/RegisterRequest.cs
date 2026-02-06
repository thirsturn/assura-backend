namespace backend.Core.DTOs.Auth;

/// <summary>
/// Request model for user registration
/// </summary>
public record RegisterRequest(
    string Firstname,
    string Lastname,
    string Email,
    string Telephone,
    string Username,
    string Password,
    string? DivisionId,
    string RoleId
);
