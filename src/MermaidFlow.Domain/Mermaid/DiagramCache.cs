namespace MermaidFlow.Domain.Mermaid;

public class DiagramCache
{
    public Guid Id { get; set; }
    public string MermaidHash { get; set; } = string.Empty;
    public string RenderedSvg { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
