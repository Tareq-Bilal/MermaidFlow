using ErrorOr;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Commands.UpdateDocument;

public record UpdateDocumentCommand(
    Guid Id,
    string Title,
    string Content,
    bool IsPublic,
    List<string> Tags) : IRequest<ErrorOr<Document>>;
