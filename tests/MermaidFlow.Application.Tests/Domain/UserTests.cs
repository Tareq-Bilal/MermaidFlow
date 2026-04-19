using FluentAssertions;
using MermaidFlow.Domain.Users;
using Xunit;

namespace MermaidFlow.Application.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsUser()
    {
        var user = User.Create("test@example.com", "hashedpassword", "John Doe");

        user.Should().NotBeNull();
        user.Id.Should().NotBe(Guid.Empty);
        user.Email.Should().Be("test@example.com");
        user.PasswordHash.Should().Be("hashedpassword");
        user.DisplayName.Should().Be("John Doe");
        user.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void Create_SetsIdToNewGuid()
    {
        var user = User.Create("test@example.com", "hash", "Name");

        user.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Update_ModifiesEmailAndDisplayName()
    {
        var user = User.Create("old@example.com", "hash", "OldName");
        var newEmail = "new@example.com";
        var newDisplayName = "NewName";

        user.Update(newEmail, newDisplayName);

        user.Email.Should().Be(newEmail);
        user.DisplayName.Should().Be(newDisplayName);
    }

    [Fact]
    public void UpdateEmail_ModifiesOnlyEmail()
    {
        var user = User.Create("old@example.com", "hash", "Name");
        var originalDisplayName = user.DisplayName;

        user.UpdateEmail("new@example.com");

        user.Email.Should().Be("new@example.com");
        user.DisplayName.Should().Be(originalDisplayName);
    }

    [Fact]
    public void UpdateDisplayName_ModifiesOnlyDisplayName()
    {
        var user = User.Create("email@example.com", "hash", "OldName");

        user.UpdateDisplayName("NewName");

        user.DisplayName.Should().Be("NewName");
        user.Email.Should().Be("email@example.com");
    }

    [Fact]
    public void PrivateConstructor_CanCreateViaFactory()
    {
        var user = User.Create("test@example.com", "hash", "Name");

        user.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_SetsCreatedAtToUtcNow()
    {
        var beforeCreate = DateTime.UtcNow;
        var user = User.Create("test@example.com", "hash", "Name");
        var afterCreate = DateTime.UtcNow;

        user.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        user.CreatedAt.Should().BeOnOrBefore(afterCreate);
    }

    [Fact]
    public void Update_WithEmptyEmail_Permitted()
    {
        var user = User.Create("old@example.com", "hash", "Name");

        user.Update("", "NewName");

        user.Email.Should().BeEmpty();
    }

    [Fact]
    public void Update_WithEmptyDisplayName_Permitted()
    {
        var user = User.Create("old@example.com", "hash", "Name");

        user.UpdateEmail("new@example.com");

        user.Email.Should().Be("new@example.com");
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithVariousDisplayNames_Permitted(string displayName)
    {
        var user = User.Create("test@example.com", "hash", displayName);

        user.DisplayName.Should().Be(displayName);
    }
}