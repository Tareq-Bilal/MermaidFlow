namespace MermaidFlow.Contracts.Documents;

public record UpdateDocumentRequest(
    string Title,
    string Content,
    bool IsPublic,
    List<string> Tags);
