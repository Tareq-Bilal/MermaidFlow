namespace MermaidFlow.Contracts.Documents;

public record CreateDocumentRequest(
    string Name,
    Guid OwnerId);
