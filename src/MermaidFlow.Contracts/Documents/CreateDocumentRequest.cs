namespace MermaidFlow.Contracts.Documents;

public record CreateDocumentRequest(
    string Title,
    string Content,
    bool IsPublic,
    List<string> Tags);
