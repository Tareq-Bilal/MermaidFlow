using ErrorOr;
using MermaidFlow.Domain.Users;
using MediatR;

namespace MermaidFlow.Application.Users.Commands.UpdateUserDisplayName;

public record UpdateUserDisplayNameCommand(Guid Id, string DisplayName) : IRequest<ErrorOr<User>>;
