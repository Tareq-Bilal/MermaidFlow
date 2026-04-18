namespace MermaidFlow.Application.Common.Interfaces;

public interface IMermaidRenderer
{
    Task<string> RenderAsync(string mermaidCode, string theme);
    Task<byte[]> RenderToPngAsync(string mermaidCode, string theme);
    Task<MermaidValidationResult> ValidateAsync(string mermaidCode);
}

public record MermaidValidationResult(bool IsValid, string? ErrorMessage);
