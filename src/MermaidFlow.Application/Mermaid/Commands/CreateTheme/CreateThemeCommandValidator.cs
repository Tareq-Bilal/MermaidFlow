using FluentValidation;

namespace MermaidFlow.Application.Mermaid.Commands.CreateTheme;

public class CreateThemeCommandValidator : AbstractValidator<CreateThemeCommand>
{
    public CreateThemeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("Theme name may only contain letters, digits, hyphens, and underscores.");
    }
}
