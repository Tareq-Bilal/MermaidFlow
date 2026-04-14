using System.Text;
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
    public async Task<IActionResult> Render(RenderMermaidRequest request)
    {
        var command = new RenderMermaidCommand(request.Code, request.Theme);

        var result = await _mediator.Send(command);

        return result.MatchFirst<IActionResult>(
            svg => File(Encoding.UTF8.GetBytes(svg), "image/svg+xml"),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(ValidateMermaidRequest request)
    {
        var query = new ValidateMermaidQuery(request.Code);

        var result = await _mediator.Send(query);

        return result.MatchFirst(
            validation => Ok(new MermaidValidationResponse(validation.IsValid, validation.ErrorMessage)),
            error => Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }
}
