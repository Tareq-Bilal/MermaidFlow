using ErrorOr;
using MediatR;

namespace MermaidFlow.Application.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string DisplayName) : IRequest<ErrorOr<AuthResult>>;
