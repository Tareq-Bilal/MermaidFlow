using ErrorOr;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Commands.CreateDocument;

public record CreateDocumentCommand(
    string Title,
    string Content,
    Guid UserId,
    bool IsPublic,
    List<string> Tags) : IRequest<ErrorOr<Document>>;
