using FluentValidation;

namespace MermaidFlow.Application.Documents.Commands.CreateDocument;

public class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
{
    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Tags)
            .NotNull();

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(50);
    }
}
