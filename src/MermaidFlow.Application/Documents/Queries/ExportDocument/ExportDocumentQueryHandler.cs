using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MediatR;

namespace MermaidFlow.Application.Documents.Queries.ExportDocument;

public class ExportDocumentQueryHandler : IRequestHandler<ExportDocumentQuery, ErrorOr<ExportDocumentResult>>
{
    private readonly IDocumentsRepository _documentsRepository;
    private readonly IDocumentExporter _documentExporter;

    public ExportDocumentQueryHandler(
        IDocumentsRepository documentsRepository,
        IDocumentExporter documentExporter)
    {
        _documentsRepository = documentsRepository;
        _documentExporter = documentExporter;
    }

    public async Task<ErrorOr<ExportDocumentResult>> Handle(ExportDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentsRepository.GetByIdAsync(request.Id);

        if (document is null)
        {
            return Error.NotFound(description: "Document not found.");
        }

        if (!document.IsPublic && document.UserId != request.RequestingUserId)
        {
            return Error.Forbidden(description: "You do not have access to this document.");
        }

        if (request.Format.Equals("html", StringComparison.OrdinalIgnoreCase))
        {
            var html = await _documentExporter.ExportToHtmlAsync(document.Content);
            return new ExportDocumentResult(html, "text/html", $"{document.Title}.html");
        }

        return Error.Validation(description: "Supported export formats: html");
    }
}
