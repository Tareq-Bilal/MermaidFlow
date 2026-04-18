using ErrorOr;
using MediatR;

namespace MermaidFlow.Application.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<ErrorOr<Deleted>>;
