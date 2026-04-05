namespace MermaidFlow.Domain.Documents;

public class Document
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; } = new();
}
