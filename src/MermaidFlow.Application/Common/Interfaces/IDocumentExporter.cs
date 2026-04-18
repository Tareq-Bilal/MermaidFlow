namespace MermaidFlow.Application.Common.Interfaces;

public interface IDocumentExporter
{
    Task<string> ExportToHtmlAsync(string markdownContent);
}
