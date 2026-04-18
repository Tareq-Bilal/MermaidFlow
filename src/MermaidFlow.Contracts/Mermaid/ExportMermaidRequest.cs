namespace MermaidFlow.Contracts.Mermaid;

public record ExportMermaidRequest(
    string Code,
    string Theme = "default",
    string Format = "svg");
