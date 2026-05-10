using ErrorOr;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.DeleteTheme;

public record DeleteThemeCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;
