using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Queries.GetDocument;

public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, ErrorOr<Document>>
{
    private readonly IDocumentsRepository _documentsRepository;

    public GetDocumentQueryHandler(IDocumentsRepository documentsRepository)
    {
        _documentsRepository = documentsRepository;
    }

    public async Task<ErrorOr<Document>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentsRepository.GetByIdAsync(request.Id);

        if (document is null)
        {
            return Error.NotFound(description: "Document not found.");
        }

        return document;
    }
}
