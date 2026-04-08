namespace MermaidFlow.Contracts.Mermaid;

public record RenderMermaidRequest(
    string Code,
    string Theme = "default");
