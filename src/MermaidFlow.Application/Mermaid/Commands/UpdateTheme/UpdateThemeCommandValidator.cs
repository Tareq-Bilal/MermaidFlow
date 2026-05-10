using FluentValidation;

namespace MermaidFlow.Application.Mermaid.Commands.UpdateTheme;

public class UpdateThemeCommandValidator : AbstractValidator<UpdateThemeCommand>
{
    public UpdateThemeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("Theme name may only contain letters, digits, hyphens, and underscores.");
    }
}
