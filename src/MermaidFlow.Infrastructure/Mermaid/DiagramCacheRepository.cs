using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Mermaid;
using MermaidFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MermaidFlow.Infrastructure.Mermaid;

public class DiagramCacheRepository : IDiagramCacheRepository
{
    private readonly MermaidFlowDbContext _dbContext;

    public DiagramCacheRepository(MermaidFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DiagramCache?> GetByHashAsync(string mermaidHash)
    {
        return await _dbContext.DiagramCaches
            .FirstOrDefaultAsync(c => c.MermaidHash == mermaidHash && c.ExpiresAt > DateTime.UtcNow);
    }

    public async Task AddAsync(DiagramCache cache)
    {
        await _dbContext.DiagramCaches.AddAsync(cache);
    }
}
