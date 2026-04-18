using FluentValidation;

namespace MermaidFlow.Application.Mermaid.Commands.ExportMermaid;

public class ExportMermaidCommandValidator : AbstractValidator<ExportMermaidCommand>
{
    private static readonly string[] AllowedFormats = ["svg", "png"];

    public ExportMermaidCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(MermaidConstants.MaxCodeLength)
            .WithMessage("Mermaid code must not exceed 50KB.");

        RuleFor(x => x.Theme)
            .NotEmpty()
            .Must(theme => MermaidConstants.AllowedThemes.Contains(theme))
            .WithMessage($"Theme must be one of: {string.Join(", ", MermaidConstants.AllowedThemes)}.");

        RuleFor(x => x.Format)
            .NotEmpty()
            .Must(format => AllowedFormats.Contains(format.ToLowerInvariant()))
            .WithMessage($"Format must be one of: {string.Join(", ", AllowedFormats)}.");
    }
}
