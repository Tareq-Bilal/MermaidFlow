using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<User>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<ErrorOr<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
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

        return user;
    }
}
