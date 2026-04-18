using System.Text;
using MermaidFlow.Application.Mermaid;
using MermaidFlow.Application.Mermaid.Commands.ExportMermaid;
using MermaidFlow.Application.Mermaid.Commands.RenderMermaid;
using MermaidFlow.Application.Mermaid.Queries.ValidateMermaid;
using MermaidFlow.Contracts.Mermaid;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MermaidFlow.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MermaidController : ControllerBase
{
    private readonly ISender _mediator;

    public MermaidController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("render")]
    [Consumes("application/json")]
    public async Task<IActionResult> Render(RenderMermaidRequest request)
    {
        var command = new RenderMermaidCommand(request.Code, request.Theme);

        var result = await _mediator.Send(command);

        return result.MatchFirst<IActionResult>(
            svg => File(Encoding.UTF8.GetBytes(svg), "image/svg+xml"),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    [HttpPost("render")]
    [Consumes("text/plain")]
    public async Task<IActionResult> RenderFromText(
        [FromBody] string code,
        [FromQuery] string theme = "default")
    {
        var command = new RenderMermaidCommand(code, theme);

        var result = await _mediator.Send(command);

        return result.MatchFirst<IActionResult>(
            svg => File(Encoding.UTF8.GetBytes(svg), "image/svg+xml"),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    [HttpPost("validate")]
    [Consumes("application/json")]
    public async Task<IActionResult> Validate(ValidateMermaidRequest request)
    {
        var query = new ValidateMermaidQuery(request.Code);

        var result = await _mediator.Send(query);

        return result.MatchFirst(
            validation => Ok(new MermaidValidationResponse(validation.IsValid, validation.ErrorMessage)),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    [HttpPost("validate")]
    [Consumes("text/plain")]
    public async Task<IActionResult> ValidateFromText([FromBody] string code)
    {
        var query = new ValidateMermaidQuery(code);

        var result = await _mediator.Send(query);

        return result.MatchFirst(
            validation => Ok(new MermaidValidationResponse(validation.IsValid, validation.ErrorMessage)),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export(ExportMermaidRequest request)
    {
        var command = new ExportMermaidCommand(request.Code, request.Theme, request.Format);

        var result = await _mediator.Send(command);

        return result.MatchFirst<IActionResult>(
            export => File(export.Data, export.ContentType, export.FileName),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    [HttpGet("themes")]
    public IActionResult GetThemes()
    {
        return Ok(MermaidConstants.AllowedThemes);
    }
}
