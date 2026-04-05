using MermaidFlow.Domain.Users;

namespace MermaidFlow.Application.Common.Interfaces;

public interface IUsersRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<bool> ExistsByEmailAsync(string email);
    void Remove(User user);
}
