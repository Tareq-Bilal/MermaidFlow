using FluentValidation;

namespace MermaidFlow.Application.Mermaid.Queries.ValidateMermaid;

public class ValidateMermaidQueryValidator : AbstractValidator<ValidateMermaidQuery>
{
    public ValidateMermaidQueryValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(MermaidConstants.MaxCodeLength)
            .WithMessage("Mermaid code must not exceed 50KB.");
    }
}
