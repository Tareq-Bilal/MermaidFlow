using System.Net;
using System.Text.RegularExpressions;
using Markdig;
using MermaidFlow.Application.Common.Interfaces;

namespace MermaidFlow.Infrastructure.Documents;

public partial class MarkdigDocumentExporter : IDocumentExporter
{
    private readonly IMermaidRenderer _mermaidRenderer;

    public MarkdigDocumentExporter(IMermaidRenderer mermaidRenderer)
    {
        _mermaidRenderer = mermaidRenderer;
    }

    public async Task<string> ExportToHtmlAsync(string markdownContent)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        var html = Markdown.ToHtml(markdownContent, pipeline);

        html = await ReplaceMermaidBlocksAsync(html);

        return WrapInHtmlTemplate(html);
    }

    private async Task<string> ReplaceMermaidBlocksAsync(string html)
    {
        var regex = MermaidCodeBlockRegex();
        var matches = regex.Matches(html);

        foreach (Match match in matches.Cast<Match>().Reverse())
        {
            var mermaidCode = WebUtility.HtmlDecode(match.Groups[1].Value.Trim());
            try
            {
                var svg = await _mermaidRenderer.RenderAsync(mermaidCode, "default");
                html = html.Remove(match.Index, match.Length).Insert(match.Index, $"<div class=\"mermaid-diagram\">{svg}</div>");
            }
            catch
            {
                // Leave the code block as-is if rendering fails
            }
        }

        return html;
    }

    private static string WrapInHtmlTemplate(string bodyHtml)
    {
        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>MermaidFlow Export</title>
                <style>
                    body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; max-width: 900px; margin: 0 auto; padding: 2rem; line-height: 1.6; color: #333; }
                    pre { background: #f5f5f5; padding: 1rem; border-radius: 4px; overflow-x: auto; }
                    code { background: #f5f5f5; padding: 0.2rem 0.4rem; border-radius: 3px; }
                    pre code { background: none; padding: 0; }
                    .mermaid-diagram { text-align: center; margin: 1.5rem 0; }
                    .mermaid-diagram svg { max-width: 100%; height: auto; }
                    table { border-collapse: collapse; width: 100%; }
                    th, td { border: 1px solid #ddd; padding: 0.5rem; text-align: left; }
                    th { background: #f5f5f5; }
                    img { max-width: 100%; }
                </style>
            </head>
            <body>
            {{bodyHtml}}
            </body>
            </html>
            """;
    }

    [GeneratedRegex(@"<pre><code class=""language-mermaid"">(.*?)</code></pre>", RegexOptions.Singleline)]
    private static partial Regex MermaidCodeBlockRegex();
}
