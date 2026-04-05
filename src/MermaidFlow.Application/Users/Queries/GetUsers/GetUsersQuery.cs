using ErrorOr;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Queries.GetUsers;

public record GetUsersQuery() : IRequest<ErrorOr<List<User>>>;
