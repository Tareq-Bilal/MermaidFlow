using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Mermaid;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.UpdateTheme;

public class UpdateThemeCommandHandler : IRequestHandler<UpdateThemeCommand, ErrorOr<Theme>>
{
    private readonly IThemeRepository _themeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateThemeCommandHandler(IThemeRepository themeRepository, IUnitOfWork unitOfWork)
    {
        _themeRepository = themeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Theme>> Handle(UpdateThemeCommand request, CancellationToken cancellationToken)
    {
        var theme = await _themeRepository.GetByIdAsync(request.Id);

        if (theme is null)
        {
            return Error.NotFound(description: "Theme not found.");
        }

        var nameConflict = await _themeRepository.GetByNameAsync(request.Name);
        if (nameConflict is not null && nameConflict.Id != request.Id)
        {
            return Error.Conflict(description: $"Theme '{request.Name}' already exists.");
        }

        theme.Name = request.Name;
        theme.IsActive = request.IsActive;

        await _unitOfWork.CommitChangesAsync();

        return theme;
    }
}
