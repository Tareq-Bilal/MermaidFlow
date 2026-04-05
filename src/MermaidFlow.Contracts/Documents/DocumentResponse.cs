namespace MermaidFlow.Contracts.Documents;

public record DocumentResponse(
    Guid Id,
    string Title,
    string Content,
    Guid UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsPublic,
    List<string> Tags);
