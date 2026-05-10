using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.DeleteTheme;

public class DeleteThemeCommandHandler : IRequestHandler<DeleteThemeCommand, ErrorOr<Deleted>>
{
    private readonly IThemeRepository _themeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteThemeCommandHandler(IThemeRepository themeRepository, IUnitOfWork unitOfWork)
    {
        _themeRepository = themeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteThemeCommand request, CancellationToken cancellationToken)
    {
        var theme = await _themeRepository.GetByIdAsync(request.Id);

        if (theme is null)
        {
            return Error.NotFound(description: "Theme not found.");
        }

        _themeRepository.Remove(theme);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}
