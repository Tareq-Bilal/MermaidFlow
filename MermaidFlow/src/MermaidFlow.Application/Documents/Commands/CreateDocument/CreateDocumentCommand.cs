using ErrorOr;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Commands.CreateDocument;

public record CreateDocumentCommand(
    string Name,
    Guid OwnerId) : IRequest<ErrorOr<Document>>;
