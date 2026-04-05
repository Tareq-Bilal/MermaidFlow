using ErrorOr;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Queries.GetUser;

public record GetUserQuery(Guid Id) : IRequest<ErrorOr<User>>;
