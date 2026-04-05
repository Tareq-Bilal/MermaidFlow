using MermaidFlow.Application.Users.Commands.CreateUser;
using MermaidFlow.Application.Users.Commands.DeleteUser;
using MermaidFlow.Application.Users.Commands.UpdateUser;
using MermaidFlow.Application.Users.Commands.UpdateUserEmail;
using MermaidFlow.Application.Users.Commands.UpdateUserDisplayName;
using MermaidFlow.Application.Users.Queries.GetUser;
using MermaidFlow.Application.Users.Queries.GetUsers;
using MermaidFlow.Contracts.Users;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MermaidFlow.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _mediator;

    public UsersController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.Password,
            request.DisplayName);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            user => CreatedAtAction(nameof(GetUser), new { id = user.Id }, ToResponse(user)),
            error => Problem(statusCode: StatusCodes.Status409Conflict, detail: error.Description));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _mediator.Send(new GetUserQuery(id));

        return result.MatchFirst(
            user => Ok(ToResponse(user)),
            error => Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description));
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _mediator.Send(new GetUsersQuery());

        return result.MatchFirst(
            users => Ok(users.Select(ToResponse).ToList()),
            error => Problem());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(id, request.Email, request.DisplayName);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            user => Ok(ToResponse(user)),
            error => Problem(
                statusCode: error.Type == ErrorOr.ErrorType.NotFound
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status409Conflict,
                detail: error.Description));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id));

        return result.MatchFirst<IActionResult>(
            _ => NoContent(),
            error => Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description));
    }

    [HttpPatch("{id:guid}/email")]
    public async Task<IActionResult> UpdateUserEmail(Guid id, UpdateUserEmailRequest request)
    {
        var result = await _mediator.Send(new UpdateUserEmailCommand(id, request.Email));

        return result.MatchFirst(
            user => Ok(ToResponse(user)),
            error => Problem(
                statusCode: error.Type == ErrorOr.ErrorType.NotFound
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status409Conflict,
                detail: error.Description));
    }

    [HttpPatch("{id:guid}/display-name")]
    public async Task<IActionResult> UpdateUserDisplayName(Guid id, UpdateUserDisplayNameRequest request)
    {
        var result = await _mediator.Send(new UpdateUserDisplayNameCommand(id, request.DisplayName));

        return result.MatchFirst(
            user => Ok(ToResponse(user)),
            error => Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description));
    }

    private static UserResponse ToResponse(MermaidFlow.Domain.Users.User user) =>
        new(user.Id, user.Email, user.DisplayName, user.CreatedAt);
}
