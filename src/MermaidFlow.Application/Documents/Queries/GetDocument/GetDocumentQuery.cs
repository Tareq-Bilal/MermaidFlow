using ErrorOr;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Queries.GetDocument;

public record GetDocumentQuery(Guid Id) : IRequest<ErrorOr<Document>>;
