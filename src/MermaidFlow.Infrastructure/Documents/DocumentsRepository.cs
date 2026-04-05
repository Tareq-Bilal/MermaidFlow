using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Documents;
using MermaidFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MermaidFlow.Infrastructure.Documents;

public class DocumentsRepository : IDocumentsRepository
{
    private readonly MermaidFlowDbContext _dbContext;

    public DocumentsRepository(MermaidFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddDocumentAsync(Document document)
    {
        await _dbContext.Documents.AddAsync(document);
    }

    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Documents.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Document>> GetAllAsync()
    {
        return await _dbContext.Documents.ToListAsync();
    }

    public void Remove(Document document)
    {
        _dbContext.Documents.Remove(document);
    }
}
