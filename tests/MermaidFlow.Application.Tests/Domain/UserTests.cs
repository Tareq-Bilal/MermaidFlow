using FluentAssertions;
using MermaidFlow.Domain.Users;
using Xunit;

namespace MermaidFlow.Application.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_ReturnsValidUser()
    {
        var user = User.Create("test@example.com", "hash", "John");
        user.Should().NotBeNull();
        user.Id.Should().NotBe(Guid.Empty);
        user.Email.Should().Be("test@example.com");
        user.DisplayName.Should().Be("John");
    }

    [Fact]
    public void Update_ModifiesProperties()
    {
        var user = User.Create("old@d.com", "hash", "Old");
        user.Update("new@d.com", "New");
        user.Email.Should().Be("new@d.com");
        user.DisplayName.Should().Be("New");
    }

    [Fact]
    public void UpdateEmail_ModifiesOnlyEmail()
    {
        var user = User.Create("old@d.com", "hash", "Name");
        user.UpdateEmail("new@d.com");
        user.Email.Should().Be("new@d.com");
    }

    [Fact]
    public void UpdateDisplayName_ModifiesOnlyName()
    {
        var user = User.Create("d.com", "hash", "Old");
        user.UpdateDisplayName("New");
        user.DisplayName.Should().Be("New");
    }
}