using ErrorOr;
using MermaidFlow.Application.Common.Helpers;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Auth;
using MediatR;

namespace MermaidFlow.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<AuthResult>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailAsync(request.Email);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Error.Unauthorized(description: "Invalid credentials.");
        }

        var authResult = _jwtTokenGenerator.GenerateToken(user);

        var refreshToken = RefreshToken.Create(
            user.Id,
            HashHelper.ComputeSha256(authResult.RefreshToken),
            authResult.RefreshTokenExpiresAt);

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.CommitChangesAsync();

        return authResult;
    }
}
