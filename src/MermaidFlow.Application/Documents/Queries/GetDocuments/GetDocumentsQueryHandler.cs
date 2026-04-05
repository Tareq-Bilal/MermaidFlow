using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Queries.GetDocuments;

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, ErrorOr<List<Document>>>
{
    private readonly IDocumentsRepository _documentsRepository;

    public GetDocumentsQueryHandler(IDocumentsRepository documentsRepository)
    {
        _documentsRepository = documentsRepository;
    }

    public async Task<ErrorOr<List<Document>>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        return await _documentsRepository.GetAllAsync();
    }
}
