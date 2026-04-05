using ErrorOr;
using MediatR;

namespace MermaidFlow.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;
