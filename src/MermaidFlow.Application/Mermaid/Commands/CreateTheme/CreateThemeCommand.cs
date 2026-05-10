using ErrorOr;
using MermaidFlow.Domain.Mermaid;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.CreateTheme;

public record CreateThemeCommand(string Name) : IRequest<ErrorOr<Theme>>;
