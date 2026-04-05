using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Documents;
using MediatR;

namespace MermaidFlow.Application.Documents.Commands.CreateDocument;

public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, ErrorOr<Document>>
{
    private readonly IDocumentsRepository _documentsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDocumentCommandHandler(IDocumentsRepository documentsRepository, IUnitOfWork unitOfWork)
    {
        _documentsRepository = documentsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Document>> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            UserId = request.UserId,
            IsPublic = request.IsPublic,
            Tags = request.Tags,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _documentsRepository.AddDocumentAsync(document);
        await _unitOfWork.CommitChangesAsync();

        return document;
    }
}
