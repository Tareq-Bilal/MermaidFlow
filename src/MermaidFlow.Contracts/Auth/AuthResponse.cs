namespace MermaidFlow.Contracts.Auth;

public record AuthResponse(
    string Token,
    string RefreshToken,
    Guid UserId,
    string Email,
    string DisplayName,
    DateTime ExpiresAt,
    DateTime RefreshTokenExpiresAt);
