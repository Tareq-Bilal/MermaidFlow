namespace MermaidFlow.Contracts.Mermaid;

public record MermaidValidationResponse(bool IsValid, string? ErrorMessage);
