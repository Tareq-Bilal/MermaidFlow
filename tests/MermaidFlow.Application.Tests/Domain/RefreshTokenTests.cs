using FluentAssertions;
using MermaidFlow.Domain.Auth;
using Xunit;

namespace MermaidFlow.Application.Tests.Domain;

public class RefreshTokenTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsToken()
    {
        var userId = Guid.NewGuid();
        var tokenHash = "hash123";
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var token = RefreshToken.Create(userId, tokenHash, expiresAt);

        token.Should().NotBeNull();
        token.Id.Should().NotBe(Guid.Empty);
        token.UserId.Should().Be(userId);
        token.TokenHash.Should().Be(tokenHash);
        token.ExpiresAt.Should().Be(expiresAt);
        token.IsRevoked.Should().BeFalse();
        token.RevokedAt.Should().BeNull();
    }

    [Fact]
    public void Revoke_SetsIsRevokedAndRevokedAt()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "hash", DateTime.UtcNow.AddDays(7));

        token.Revoke();

        token.IsRevoked.Should().BeTrue();
        token.RevokedAt.Should().NotBeNull();
    }

    [Fact]
    public void Revoke_AlreadyRevoked_CanRevokeAgain()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "hash", DateTime.UtcNow.AddDays(7));
        token.Revoke();

        token.Revoke();

        token.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public void Create_SetsCreatedAtToUtcNow()
    {
        var beforeCreate = DateTime.UtcNow;
        var token = RefreshToken.Create(Guid.NewGuid(), "hash", DateTime.UtcNow.AddDays(7));
        var afterCreate = DateTime.UtcNow;

        token.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        token.CreatedAt.Should().BeOnOrBefore(afterCreate);
    }

    [Fact]
    public void Create_DefaultId_IsNotEmpty()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "hash", DateTime.UtcNow.AddDays(7));

        token.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithExpiredDate_Passes()
    {
        var userId = Guid.NewGuid();
        var tokenHash = "hash123";
        var expiredDate = DateTime.UtcNow.AddMinutes(-1);

        var token = RefreshToken.Create(userId, tokenHash, expiredDate);

        token.ExpiresAt.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void Revoke_UpdatesRevokedAtToCurrentTime()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "hash", DateTime.UtcNow.AddDays(7));

        token.Revoke();

        token.RevokedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}