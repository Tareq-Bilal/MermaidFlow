using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ErrorOr<User>>
{
    private readonly IUsersRepository _usersRepository;

    public GetUserQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<ErrorOr<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            return Error.NotFound(description: "User not found.");
        }

        return user;
    }
}
