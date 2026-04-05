namespace MermaidFlow.Contracts.Users;

public record CreateUserRequest(
    string Email,
    string Password,
    string DisplayName);
