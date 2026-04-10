using System.Text.RegularExpressions;

namespace MermaidFlow.Application.Common.Helpers;

public static partial class MermaidContentFormatter
{
    /// <summary>
    /// Takes raw Mermaid content (potentially escaped from JSON) and returns
    /// clean, valid, ready-to-render Mermaid syntax.
    /// </summary>
    public static string Format(string rawContent)
    {
        if (string.IsNullOrWhiteSpace(rawContent))
            return rawContent;

        var result = rawContent;

        // 1. Unescape double-escaped sequences first (\\n -> \n, \\" -> \")
        result = result.Replace("\\\\n", "\n");
        result = result.Replace("\\\\\"", "\"");

        // 2. Unescape single-escaped sequences (\n -> newline, \" -> ")
        result = result.Replace("\\n", "\n");
        result = result.Replace("\\\"", "\"");

        // 3. Strip markdown code block wrappers if present
        result = MarkdownMermaidBlockRegex().Replace(result, "$1");

        // 4. Trim leading/trailing empty lines while preserving internal indentation
        result = result.Trim('\r', '\n');

        return result;
    }

    [GeneratedRegex(@"^\s*```\s*mermaid\s*\n([\s\S]*?)\n\s*```\s*$", RegexOptions.Multiline)]
    private static partial Regex MarkdownMermaidBlockRegex();
}
