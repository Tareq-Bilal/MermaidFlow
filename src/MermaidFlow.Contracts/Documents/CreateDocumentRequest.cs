namespace MermaidFlow.Contracts.Documents;

public record CreateDocumentRequest(
    string Title,
    string Content,
    Guid UserId,
    bool IsPublic,
    List<string> Tags);
