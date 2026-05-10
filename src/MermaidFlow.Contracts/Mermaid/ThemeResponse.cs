namespace MermaidFlow.Contracts.Mermaid;

public record ThemeResponse(
    Guid Id,
    string Name,
    bool IsActive,
    DateTime CreatedAt);
