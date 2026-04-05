using ErrorOr;
using MediatR;

namespace MermaidFlow.Application.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<ErrorOr<AuthResult>>;
