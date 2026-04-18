using ErrorOr;
using MediatR;

namespace MermaidFlow.Application.Documents.Queries.ExportDocument;

public record ExportDocumentQuery(
    Guid Id,
    Guid RequestingUserId,
    string Format) : IRequest<ErrorOr<ExportDocumentResult>>;

public record ExportDocumentResult(string Content, string ContentType, string FileName);
