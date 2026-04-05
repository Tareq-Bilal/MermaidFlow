using MermaidFlow.Application.Documents.Commands.CreateDocument;
using MermaidFlow.Application.Documents.Queries.GetDocument;
using MermaidFlow.Contracts.Documents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MermaidFlow.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ISender _mediator;

    public DocumentsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocument(CreateDocumentRequest request)
    {
        var command = new CreateDocumentCommand(
            request.Title,
            request.Content,
            request.UserId,
            request.IsPublic,
            request.Tags);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            document => Ok(ToResponse(document)),
            error => Problem());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDocument(Guid id)
    {
        var query = new GetDocumentQuery(id);

        var result = await _mediator.Send(query);

        return result.MatchFirst(
            document => Ok(ToResponse(document)),
            error => Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description));
    }

    private static DocumentResponse ToResponse(MermaidFlow.Domain.Documents.Document document) =>
        new(document.Id, document.Title, document.Content, document.UserId,
            document.CreatedAt, document.UpdatedAt, document.IsPublic, document.Tags);
}
