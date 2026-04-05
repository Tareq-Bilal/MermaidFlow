using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Commands.CreateDocument;

public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, ErrorOr<Document>>
{
    private readonly IDocumentsRepository _documentsRepository;

    public CreateDocumentCommandHandler(IDocumentsRepository documentsRepository)
    {
        _documentsRepository = documentsRepository;
    }

    public async Task<ErrorOr<Document>> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        // 1. Create the domain entity
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerId = request.OwnerId,
        };

        // 2. Persist to database
        await _documentsRepository.AddDocumentAsync(document);

        // 3. Return result
        return document;
    }
}
