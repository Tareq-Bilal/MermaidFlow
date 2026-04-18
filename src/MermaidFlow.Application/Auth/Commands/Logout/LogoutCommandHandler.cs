using ErrorOr;
using MermaidFlow.Application.Common.Helpers;
using MermaidFlow.Application.Common.Interfaces;
using MediatR;

namespace MermaidFlow.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ErrorOr<Deleted>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = HashHelper.ComputeSha256(request.RefreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (storedToken is null)
        {
            return Result.Deleted;
        }

        storedToken.Revoke();
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}
