using MermaidFlow.Domain.Mermaid;

namespace MermaidFlow.Application.Common.Interfaces;

public interface IDiagramCacheRepository
{
    Task<DiagramCache?> GetByHashAsync(string mermaidHash);
    Task AddAsync(DiagramCache cache);
}
