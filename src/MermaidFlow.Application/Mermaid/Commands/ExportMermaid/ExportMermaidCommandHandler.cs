using System.Text;
using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.ExportMermaid;

public class ExportMermaidCommandHandler : IRequestHandler<ExportMermaidCommand, ErrorOr<ExportMermaidResult>>
{
    private readonly IMermaidRenderer _mermaidRenderer;

    public ExportMermaidCommandHandler(IMermaidRenderer mermaidRenderer)
    {
        _mermaidRenderer = mermaidRenderer;
    }

    public async Task<ErrorOr<ExportMermaidResult>> Handle(ExportMermaidCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Format.Equals("png", StringComparison.OrdinalIgnoreCase))
            {
                var pngBytes = await _mermaidRenderer.RenderToPngAsync(request.Code, request.Theme);
                return new ExportMermaidResult(pngBytes, "image/png", "diagram.png");
            }

            var svg = await _mermaidRenderer.RenderAsync(request.Code, request.Theme);
            return new ExportMermaidResult(Encoding.UTF8.GetBytes(svg), "image/svg+xml", "diagram.svg");
        }
        catch (Exception ex)
        {
            return Error.Failure("Mermaid.ExportFailed", ex.Message);
        }
    }
}
