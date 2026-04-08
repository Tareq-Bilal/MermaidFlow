using FluentValidation;

namespace MermaidFlow.Application.Mermaid.Commands.RenderMermaid;

public class RenderMermaidCommandValidator : AbstractValidator<RenderMermaidCommand>
{
    public RenderMermaidCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(MermaidConstants.MaxCodeLength)
            .WithMessage("Mermaid code must not exceed 50KB.");

        RuleFor(x => x.Theme)
            .NotEmpty()
            .Must(theme => MermaidConstants.AllowedThemes.Contains(theme))
            .WithMessage($"Theme must be one of: {string.Join(", ", MermaidConstants.AllowedThemes)}.");
    }
}
