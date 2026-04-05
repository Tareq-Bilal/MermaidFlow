namespace MermaidFlow.Contracts.Users;

public record UpdateUserRequest(
    string Email,
    string DisplayName);
