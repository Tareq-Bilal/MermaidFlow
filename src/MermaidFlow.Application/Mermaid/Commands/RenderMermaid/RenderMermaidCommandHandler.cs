using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.RenderMermaid;

public class RenderMermaidCommandHandler : IRequestHandler<RenderMermaidCommand, ErrorOr<string>>
{
    private readonly IMermaidRenderer _mermaidRenderer;

    public RenderMermaidCommandHandler(IMermaidRenderer mermaidRenderer)
    {
        _mermaidRenderer = mermaidRenderer;
    }

    public async Task<ErrorOr<string>> Handle(RenderMermaidCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var svg = await _mermaidRenderer.RenderAsync(request.Code, request.Theme);
            return svg;
        }
        catch (Exception ex)
        {
            return Error.Failure("Mermaid.RenderFailed", ex.Message);
        }
    }
}
