using System.Security.Claims;
using System.Text;
using MermaidFlow.Application.Documents.Commands.CreateDocument;
using MermaidFlow.Application.Documents.Commands.UpdateDocument;
using MermaidFlow.Application.Documents.Commands.DeleteDocument;
using MermaidFlow.Application.Documents.Queries.GetDocument;
using MermaidFlow.Application.Documents.Queries.GetDocuments;
using MermaidFlow.Application.Documents.Queries.GetPublicDocuments;
using MermaidFlow.Application.Documents.Queries.ExportDocument;
using MermaidFlow.Application.Common.Helpers;
using MermaidFlow.Contracts.Documents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreateDocumentCommand(
            request.Title,
            request.Content,
            userId,
            request.IsPublic,
            request.Tags);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            document => CreatedAtAction(nameof(GetDocument), new { id = document.Id }, ToResponse(document)),
            error => Problem());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDocument(Guid id)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetDocumentQuery(id, requestingUserId);

        var result = await _mediator.Send(query);

        return result.MatchFirst(
            document => Ok(ToResponse(document)),
            error => error.Type == ErrorOr.ErrorType.Forbidden
                ? Problem(statusCode: StatusCodes.Status403Forbidden, detail: error.Description)
                : Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description));
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new GetDocumentsQuery(userId));

        return result.MatchFirst(
            documents => Ok(documents.Select(ToResponse).ToList()),
            error => Problem());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDocument(Guid id, UpdateDocumentRequest request)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new UpdateDocumentCommand(
            id,
            requestingUserId,
            request.Title,
            request.Content,
            request.IsPublic,
            request.Tags);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            document => Ok(ToResponse(document)),
            error => error.Type == ErrorOr.ErrorType.Forbidden
                ? Problem(statusCode: StatusCodes.Status403Forbidden, detail: error.Description)
                : Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new DeleteDocumentCommand(id, requestingUserId));

        return result.MatchFirst<IActionResult>(
            _ => NoContent(),
            error => error.Type == ErrorOr.ErrorType.Forbidden
                ? Problem(statusCode: StatusCodes.Status403Forbidden, detail: error.Description)
                : Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description));
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicDocuments()
    {
        var result = await _mediator.Send(new GetPublicDocumentsQuery());

        return result.MatchFirst(
            documents => Ok(documents.Select(ToResponse).ToList()),
            error => Problem());
    }

    [HttpGet("{id:guid}/export")]
    public async Task<IActionResult> ExportDocument(Guid id, [FromQuery] string format = "html")
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new ExportDocumentQuery(id, requestingUserId, format));

        return result.MatchFirst<IActionResult>(
            export => File(Encoding.UTF8.GetBytes(export.Content), export.ContentType, export.FileName),
            error => error.Type == ErrorOr.ErrorType.Forbidden
                ? Problem(statusCode: StatusCodes.Status403Forbidden, detail: error.Description)
                : error.Type == ErrorOr.ErrorType.NotFound
                    ? Problem(statusCode: StatusCodes.Status404NotFound, detail: error.Description)
                    : Problem(statusCode: StatusCodes.Status400BadRequest, detail: error.Description));
    }

    private static DocumentResponse ToResponse(MermaidFlow.Domain.Documents.Document document) =>
        new(document.Id, document.Title, MermaidContentFormatter.Format(document.Content), document.UserId,
            document.CreatedAt, document.UpdatedAt, document.IsPublic, document.Tags);
}
