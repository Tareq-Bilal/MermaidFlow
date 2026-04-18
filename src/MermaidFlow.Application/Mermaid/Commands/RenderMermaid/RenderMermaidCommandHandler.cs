using ErrorOr;
using MermaidFlow.Application.Common.Helpers;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Mermaid;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.RenderMermaid;

public class RenderMermaidCommandHandler : IRequestHandler<RenderMermaidCommand, ErrorOr<string>>
{
    private readonly IMermaidRenderer _mermaidRenderer;
    private readonly IDiagramCacheRepository _diagramCacheRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RenderMermaidCommandHandler(
        IMermaidRenderer mermaidRenderer,
        IDiagramCacheRepository diagramCacheRepository,
        IUnitOfWork unitOfWork)
    {
        _mermaidRenderer = mermaidRenderer;
        _diagramCacheRepository = diagramCacheRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<string>> Handle(RenderMermaidCommand request, CancellationToken cancellationToken)
    {
        var cacheKey = HashHelper.ComputeSha256($"{request.Code}|{request.Theme}");
        var cached = await _diagramCacheRepository.GetByHashAsync(cacheKey);

        if (cached is not null && cached.ExpiresAt > DateTime.UtcNow)
        {
            return cached.RenderedSvg;
        }

        try
        {
            var svg = await _mermaidRenderer.RenderAsync(request.Code, request.Theme);

            var diagramCache = new DiagramCache
            {
                Id = Guid.NewGuid(),
                MermaidHash = cacheKey,
                RenderedSvg = svg,
                Theme = request.Theme,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
            };

            await _diagramCacheRepository.AddAsync(diagramCache);
            await _unitOfWork.CommitChangesAsync();

            return svg;
        }
        catch (Exception ex)
        {
            return Error.Failure("Mermaid.RenderFailed", ex.Message);
        }
    }
}
