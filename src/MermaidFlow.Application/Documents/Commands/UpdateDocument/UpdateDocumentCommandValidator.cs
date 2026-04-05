using FluentValidation;

namespace MermaidFlow.Application.Documents.Commands.UpdateDocument;

public class UpdateDocumentCommandValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty();

        RuleFor(x => x.Tags)
            .NotNull();

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(50);
    }
}
