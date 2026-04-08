using ErrorOr;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Queries.GetDocuments;

public record GetDocumentsQuery(Guid UserId) : IRequest<ErrorOr<List<Document>>>;
