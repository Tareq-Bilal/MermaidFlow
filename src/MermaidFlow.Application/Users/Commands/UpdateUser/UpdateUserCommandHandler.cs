using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<User>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<User>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            return Error.NotFound(description: "User not found.");
        }

        if (user.Email != request.Email && await _usersRepository.ExistsByEmailAsync(request.Email))
        {
            return Error.Conflict(description: "A user with this email already exists.");
        }

        user.Update(request.Email, request.DisplayName);
        await _unitOfWork.CommitChangesAsync();

        return user;
    }
}
