using MermaidFlow.Application.Common.Interfaces;
using MermaidFlow.Domain.Users;
using MermaidFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MermaidFlow.Infrastructure.Users;

public class UsersRepository : IUsersRepository
{
    private readonly MermaidFlowDbContext _dbContext;

    public UsersRepository(MermaidFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == email);
    }

    public void Remove(User user)
    {
        _dbContext.Users.Remove(user);
    }
}
