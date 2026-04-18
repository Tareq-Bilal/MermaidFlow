using ErrorOr;
using MediatR;

namespace MermaidFlow.Application.Documents.Commands.DeleteDocument;

public record DeleteDocumentCommand(Guid Id, Guid RequestingUserId) : IRequest<ErrorOr<Deleted>>;
