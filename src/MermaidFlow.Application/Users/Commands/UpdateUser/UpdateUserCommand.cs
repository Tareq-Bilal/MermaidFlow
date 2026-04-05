using ErrorOr;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string Email,
    string DisplayName) : IRequest<ErrorOr<User>>;
