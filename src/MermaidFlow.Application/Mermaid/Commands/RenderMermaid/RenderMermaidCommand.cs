using ErrorOr;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.RenderMermaid;

public record RenderMermaidCommand(
    string Code,
    string Theme) : IRequest<ErrorOr<string>>;
