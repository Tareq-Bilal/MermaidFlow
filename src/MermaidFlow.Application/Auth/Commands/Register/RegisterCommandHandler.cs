using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthResult>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<AuthResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _usersRepository.ExistsByEmailAsync(request.Email))
        {
            return Error.Conflict(description: "A user with this email already exists.");
        }

        var user = User.Create(
            request.Email,
            _passwordHasher.Hash(request.Password),
            request.DisplayName);

        await _usersRepository.AddAsync(user);
        await _unitOfWork.CommitChangesAsync();

        return _jwtTokenGenerator.GenerateToken(user);
    }
}
