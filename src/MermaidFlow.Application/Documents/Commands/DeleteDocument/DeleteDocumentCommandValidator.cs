using FluentValidation;

namespace MermaidFlow.Application.Documents.Commands.DeleteDocument;

public class DeleteDocumentCommandValidator : AbstractValidator<DeleteDocumentCommand>
{
    public DeleteDocumentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
