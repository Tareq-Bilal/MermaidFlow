using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, ErrorOr<List<User>>>
{
    private readonly IUsersRepository _usersRepository;

    public GetUsersQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<ErrorOr<List<User>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _usersRepository.GetAllAsync();
        return users;
    }
}
