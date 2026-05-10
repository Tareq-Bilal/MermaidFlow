using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Mermaid;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Queries.GetTheme;

public class GetThemeQueryHandler : IRequestHandler<GetThemeQuery, ErrorOr<Theme>>
{
    private readonly IThemeRepository _themeRepository;

    public GetThemeQueryHandler(IThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
    }

    public async Task<ErrorOr<Theme>> Handle(GetThemeQuery request, CancellationToken cancellationToken)
    {
        var theme = await _themeRepository.GetByIdAsync(request.Id);

        if (theme is null)
        {
            return Error.NotFound(description: "Theme not found.");
        }

        return theme;
    }
}
