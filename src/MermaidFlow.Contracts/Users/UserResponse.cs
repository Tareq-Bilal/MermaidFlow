namespace MermaidFlow.Contracts.Users;

public record UserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    DateTime CreatedAt);
