using ErrorOr;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string Password,
    string DisplayName) : IRequest<ErrorOr<User>>;
