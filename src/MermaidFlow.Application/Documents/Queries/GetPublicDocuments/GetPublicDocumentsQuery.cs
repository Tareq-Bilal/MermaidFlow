using ErrorOr;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Queries.GetPublicDocuments;

public record GetPublicDocumentsQuery() : IRequest<ErrorOr<List<Document>>>;
