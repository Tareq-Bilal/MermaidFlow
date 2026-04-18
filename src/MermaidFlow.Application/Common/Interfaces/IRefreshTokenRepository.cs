using MermaidFlow.Domain.Auth;

namespace MermaidFlow.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
}
