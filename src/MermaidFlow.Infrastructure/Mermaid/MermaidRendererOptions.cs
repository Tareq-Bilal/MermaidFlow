namespace MermaidFlow.Infrastructure.Mermaid;

public sealed class MermaidRendererOptions
{
    public const string SectionName = "MermaidRenderer";

    /// <summary>Number of Playwright pages to keep in the pool. Default: 4.</summary>
    public int PoolSize { get; set; } = 4;

    /// <summary>Timeout in milliseconds for a single render operation. Default: 15000.</summary>
    public int RenderTimeoutMs { get; set; } = 15_000;

    /// <summary>URL or local path to mermaid.min.js. Default: jsDelivr CDN.</summary>
    public string MermaidJsUrl { get; set; } =
        "https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.min.js";
}
