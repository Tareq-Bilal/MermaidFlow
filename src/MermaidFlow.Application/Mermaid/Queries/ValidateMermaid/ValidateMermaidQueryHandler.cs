using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Queries.ValidateMermaid;

public class ValidateMermaidQueryHandler : IRequestHandler<ValidateMermaidQuery, ErrorOr<MermaidValidationResult>>
{
    private readonly IMermaidRenderer _mermaidRenderer;

    public ValidateMermaidQueryHandler(IMermaidRenderer mermaidRenderer)
    {
        _mermaidRenderer = mermaidRenderer;
    }

    public async Task<ErrorOr<MermaidValidationResult>> Handle(ValidateMermaidQuery request, CancellationToken cancellationToken)
    {
        var result = await _mermaidRenderer.ValidateAsync(request.Code);
        return result;
    }
}
