namespace MermaidFlow.Domain.Users;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private User() { } // Required by EF Core

    public static User Create(string email, string passwordHash, string displayName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            DisplayName = displayName,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Update(string email, string displayName)
    {
        Email = email;
        DisplayName = displayName;
    }

    public void UpdateEmail(string email)
    {
        Email = email;
    }

    public void UpdateDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
}
