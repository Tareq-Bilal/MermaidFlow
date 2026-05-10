using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Mermaid;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.CreateTheme;

public class CreateThemeCommandHandler : IRequestHandler<CreateThemeCommand, ErrorOr<Theme>>
{
    private readonly IThemeRepository _themeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateThemeCommandHandler(IThemeRepository themeRepository, IUnitOfWork unitOfWork)
    {
        _themeRepository = themeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Theme>> Handle(CreateThemeCommand request, CancellationToken cancellationToken)
    {
        if (await _themeRepository.ExistsByNameAsync(request.Name))
        {
            return Error.Conflict(description: $"Theme '{request.Name}' already exists.");
        }

        var theme = new Theme
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        await _themeRepository.AddAsync(theme);
        await _unitOfWork.CommitChangesAsync();

        return theme;
    }
}
