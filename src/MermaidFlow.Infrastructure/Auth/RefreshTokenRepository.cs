using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Auth;
using MermaidFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MermaidFlow.Infrastructure.Auth;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MermaidFlowDbContext _dbContext;

    public RefreshTokenRepository(MermaidFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _dbContext.RefreshTokens.AddAsync(token);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
    }
}
