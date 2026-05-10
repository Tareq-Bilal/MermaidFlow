using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Mermaid;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Queries.GetThemes;

public class GetThemesQueryHandler : IRequestHandler<GetThemesQuery, ErrorOr<List<Theme>>>
{
    private readonly IThemeRepository _themeRepository;

    public GetThemesQueryHandler(IThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
    }

    public async Task<ErrorOr<List<Theme>>> Handle(GetThemesQuery request, CancellationToken cancellationToken)
    {
        return await _themeRepository.GetAllAsync();
    }
}
