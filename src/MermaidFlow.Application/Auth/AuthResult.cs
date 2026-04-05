namespace MermaidFlow.Application.Auth;

public record AuthResult(
    string Token,
    Guid UserId,
    string Email,
    string DisplayName,
    DateTime ExpiresAt);
