using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Commands.UpdateDocument;

public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, ErrorOr<Document>>
{
    private readonly IDocumentsRepository _documentsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDocumentCommandHandler(IDocumentsRepository documentsRepository, IUnitOfWork unitOfWork)
    {
        _documentsRepository = documentsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Document>> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentsRepository.GetByIdAsync(request.Id);

        if (document is null)
        {
            return Error.NotFound(description: "Document not found.");
        }

        document.Title = request.Title;
        document.Content = request.Content;
        document.IsPublic = request.IsPublic;
        document.Tags = request.Tags;
        document.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.CommitChangesAsync();

        return document;
    }
}
