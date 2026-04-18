namespace MermaidFlow.Application.Auth;

public record AuthResult(
    string Token,
    string RefreshToken,
    Guid UserId,
    string Email,
    string DisplayName,
    DateTime ExpiresAt,
    DateTime RefreshTokenExpiresAt);
