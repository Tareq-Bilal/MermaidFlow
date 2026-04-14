using System.Xml.Linq;

namespace MermaidFlow.Infrastructure.Mermaid;

/// <summary>
/// Cleans and fixes common issues in Mermaid-generated SVG output:
/// escaped quotes, hardcoded dimensions, and CSS ID collisions.
/// </summary>
internal static class SvgCleaner
{
    private static readonly XNamespace SvgNs = "http://www.w3.org/2000/svg";

    public static string Clean(string rawSvg)
    {
        // Fix escaped quotes that break XML parsing
        var cleaned = rawSvg.Replace("\\\"", "\"");

        XDocument doc;
        try
        {
            doc = XDocument.Parse(cleaned);
        }
        catch
        {
            // If it still can't be parsed, return the quote-fixed string as-is
            return cleaned;
        }

        var root = doc.Root;
        if (root is null)
            return cleaned;

        MakeResponsive(root);
        AssignUniqueId(root);

        return doc.ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Replaces hardcoded max-width (e.g. "max-width: 228.765625px;") with 100%
    /// so the SVG scales to its container.
    /// </summary>
    private static void MakeResponsive(XElement root)
    {
        var style = root.Attribute("style");
        if (style is not null && style.Value.Contains("max-width"))
        {
            style.Value = "max-width: 100%;";
        }
    }

    /// <summary>
    /// Replaces the default "mermaid-diagram" ID with a unique one to prevent
    /// CSS bleeding when multiple diagrams are on the same page.
    /// </summary>
    private static void AssignUniqueId(XElement root)
    {
        var idAttr = root.Attribute("id");
        if (idAttr is null)
            return;

        var oldId = idAttr.Value;
        var uniqueId = $"mermaid-{Guid.NewGuid():N}"[..16];
        idAttr.Value = uniqueId;

        // Update the embedded <style> block to reference the new ID
        var styleNode = root.Descendants(SvgNs + "style").FirstOrDefault();
        if (styleNode is not null)
        {
            styleNode.Value = styleNode.Value.Replace($"#{oldId}", $"#{uniqueId}");
        }
    }
}
