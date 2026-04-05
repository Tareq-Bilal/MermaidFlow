using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Commands.UpdateUserDisplayName;

public class UpdateUserDisplayNameCommandHandler : IRequestHandler<UpdateUserDisplayNameCommand, ErrorOr<User>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserDisplayNameCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork)
    {
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<User>> Handle(UpdateUserDisplayNameCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            return Error.NotFound(description: "User not found.");
        }

        user.UpdateDisplayName(request.DisplayName);
        await _unitOfWork.CommitChangesAsync();

        return user;
    }
}
