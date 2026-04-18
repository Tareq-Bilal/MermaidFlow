using ErrorOr;
using MermaidFlow.Application.Common.Helpers;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Auth;
using MediatR;

namespace MermaidFlow.Application.Auth.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, ErrorOr<AuthResult>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUsersRepository usersRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _usersRepository = usersRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AuthResult>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = HashHelper.ComputeSha256(request.RefreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Error.Unauthorized(description: "Invalid or expired refresh token.");
        }

        var user = await _usersRepository.GetByIdAsync(storedToken.UserId);

        if (user is null)
        {
            return Error.Unauthorized(description: "User not found.");
        }

        storedToken.Revoke();

        var authResult = _jwtTokenGenerator.GenerateToken(user);

        var newRefreshToken = RefreshToken.Create(
            user.Id,
            HashHelper.ComputeSha256(authResult.RefreshToken),
            authResult.RefreshTokenExpiresAt);

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _unitOfWork.CommitChangesAsync();

        return authResult;
    }
}
