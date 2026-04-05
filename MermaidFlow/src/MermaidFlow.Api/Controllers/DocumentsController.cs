using MermaidFlow.Application.Documents.Commands.CreateDocument;
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
            request.Name,
            request.OwnerId);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            document => Ok(new DocumentResponse(document.Id, document.Name)),
            error => Problem());
    }
}
