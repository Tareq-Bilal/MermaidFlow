using ErrorOr;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Queries.GetDocuments;

public record GetDocumentsQuery : IRequest<ErrorOr<List<Document>>>;
