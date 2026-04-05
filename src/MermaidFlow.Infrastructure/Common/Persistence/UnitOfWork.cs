using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Infrastructure.Persistence;

namespace MermaidFlow.Infrastructure.Common.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly MermaidFlowDbContext _dbContext;

    public UnitOfWork(MermaidFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CommitChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
