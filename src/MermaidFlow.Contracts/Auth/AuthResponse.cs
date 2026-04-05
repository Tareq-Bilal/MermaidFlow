namespace MermaidFlow.Contracts.Auth;

public record AuthResponse(
    string Token,
    Guid UserId,
    string Email,
    string DisplayName,
    DateTime ExpiresAt);
