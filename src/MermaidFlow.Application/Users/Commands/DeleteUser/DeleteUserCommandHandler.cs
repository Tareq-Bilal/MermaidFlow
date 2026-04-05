using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MediatR;

namespace MermaidFlow.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ErrorOr<Deleted>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            return Error.NotFound(description: "User not found.");
        }

        _usersRepository.Remove(user);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}
