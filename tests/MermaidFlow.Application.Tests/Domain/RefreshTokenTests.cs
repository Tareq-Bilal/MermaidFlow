using FluentAssertions;
using MermaidFlow.Domain.Auth;
using Xunit;

namespace MermaidFlow.Application.Tests.Domain;

public class RefreshTokenTests
{
    [Fact]
    public void Create_ReturnsValidToken()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "hash", DateTime.UtcNow.AddDays(7));
        token.Should().NotBeNull();
        token.Id.Should().NotBe(Guid.Empty);
        token.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void Revoke_SetsIsRevoked()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "hash", DateTime.UtcNow.AddDays(7));
        token.Revoke();
        token.IsRevoked.Should().BeTrue();
        token.RevokedAt.Should().NotBeNull();
    }
}