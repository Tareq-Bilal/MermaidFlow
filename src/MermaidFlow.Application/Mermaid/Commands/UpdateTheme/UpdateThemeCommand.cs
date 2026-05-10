using ErrorOr;
using MermaidFlow.Domain.Mermaid;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.UpdateTheme;

public record UpdateThemeCommand(Guid Id, string Name, bool IsActive) : IRequest<ErrorOr<Theme>>;
