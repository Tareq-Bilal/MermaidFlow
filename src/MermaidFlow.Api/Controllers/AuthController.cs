using MermaidFlow.Application.Auth;
using MermaidFlow.Application.Auth.Commands.Login;
using MermaidFlow.Application.Auth.Commands.Logout;
using MermaidFlow.Application.Auth.Commands.Refresh;
using MermaidFlow.Application.Auth.Commands.Register;
using MermaidFlow.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MermaidFlow.Api.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.DisplayName);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            auth => Ok(ToResponse(auth)),
            error => Problem(statusCode: StatusCodes.Status409Conflict, detail: error.Description));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            auth => Ok(ToResponse(auth)),
            error => Problem(statusCode: StatusCodes.Status401Unauthorized, detail: error.Description));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var command = new RefreshCommand(request.RefreshToken);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            auth => Ok(ToResponse(auth)),
            error => Problem(statusCode: StatusCodes.Status401Unauthorized, detail: error.Description));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest request)
    {
        var command = new LogoutCommand(request.RefreshToken);

        var result = await _mediator.Send(command);

        return result.MatchFirst<IActionResult>(
            _ => NoContent(),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    private static AuthResponse ToResponse(AuthResult auth) =>
        new(auth.Token, auth.RefreshToken, auth.UserId, auth.Email, auth.DisplayName, auth.ExpiresAt, auth.RefreshTokenExpiresAt);
}
