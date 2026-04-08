using MermaidFlow.Domain.Documents;

namespace MermaidFlow.Application.Common.Interfaces;

public interface IDocumentsRepository
{
    Task AddDocumentAsync(Document document);
    Task<Document?> GetByIdAsync(Guid id);
    Task<List<Document>> GetAllAsync();
    Task<List<Document>> GetByUserIdAsync(Guid userId);
    void Remove(Document document);
}
