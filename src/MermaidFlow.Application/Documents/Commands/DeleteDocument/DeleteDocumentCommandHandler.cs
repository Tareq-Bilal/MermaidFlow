using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MediatR;

namespace MermaidFlow.Application.Documents.Commands.DeleteDocument;

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, ErrorOr<Deleted>>
{
    private readonly IDocumentsRepository _documentsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDocumentCommandHandler(IDocumentsRepository documentsRepository, IUnitOfWork unitOfWork)
    {
        _documentsRepository = documentsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentsRepository.GetByIdAsync(request.Id);

        if (document is null)
        {
            return Error.NotFound(description: "Document not found.");
        }

        _documentsRepository.Remove(document);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}
