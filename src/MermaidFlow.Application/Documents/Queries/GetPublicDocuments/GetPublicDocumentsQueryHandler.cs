using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Queries.GetPublicDocuments;

public class GetPublicDocumentsQueryHandler : IRequestHandler<GetPublicDocumentsQuery, ErrorOr<List<Document>>>
{
    private readonly IDocumentsRepository _documentsRepository;

    public GetPublicDocumentsQueryHandler(IDocumentsRepository documentsRepository)
    {
        _documentsRepository = documentsRepository;
    }

    public async Task<ErrorOr<List<Document>>> Handle(GetPublicDocumentsQuery request, CancellationToken cancellationToken)
    {
        return await _documentsRepository.GetPublicDocumentsAsync();
    }
}
