using ErrorOr;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Commands.UpdateUserEmail;

public record UpdateUserEmailCommand(Guid Id, string Email) : IRequest<ErrorOr<User>>;
