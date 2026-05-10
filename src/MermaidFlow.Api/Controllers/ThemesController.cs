using MermaidFlow.Application.Mermaid.Commands.CreateTheme;
using MermaidFlow.Application.Mermaid.Commands.DeleteTheme;
using MermaidFlow.Application.Mermaid.Commands.UpdateTheme;
using MermaidFlow.Application.Mermaid.Queries.GetTheme;
using MermaidFlow.Application.Mermaid.Queries.GetThemes;
using MermaidFlow.Contracts.Mermaid;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MermaidFlow.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ThemesController : ControllerBase
{
    private readonly ISender _mediator;

    public ThemesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetThemesQuery());

        return result.MatchFirst(
            themes => Ok(themes.Select(ToResponse)),
            error => Problem(statusCode: StatusCodes.Status500InternalServerError, detail: error.Description));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetThemeQuery(id));

        return result.MatchFirst(
            theme => Ok(ToResponse(theme)),
            error => Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateThemeRequest request)
    {
        var command = new CreateThemeCommand(request.Name);
        var result = await _mediator.Send(command);

        return result.MatchFirst(
            theme => CreatedAtAction(nameof(GetById), new { id = theme.Id }, ToResponse(theme)),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateThemeRequest request)
    {
        var command = new UpdateThemeCommand(id, request.Name, request.IsActive);
        var result = await _mediator.Send(command);

        return result.MatchFirst(
            theme => Ok(ToResponse(theme)),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteThemeCommand(id));

        return result.MatchFirst<IActionResult>(
            _ => NoContent(),
            error => Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description));
    }

    private static ThemeResponse ToResponse(MermaidFlow.Domain.Mermaid.Theme theme) =>
        new(theme.Id, theme.Name, theme.IsActive, theme.CreatedAt);
}
