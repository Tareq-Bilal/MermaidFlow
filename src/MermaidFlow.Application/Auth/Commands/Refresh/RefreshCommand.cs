using ErrorOr;
using MermaidFlow.Application.Auth;
using MediatR;

namespace MermaidFlow.Application.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<ErrorOr<AuthResult>>;
