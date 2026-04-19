using FluentAssertions;
using MermaidFlow.Domain.Mermaid;
using Xunit;

namespace MermaidFlow.Application.Tests.Domain;

public class DiagramCacheTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsCache()
    {
        var cache = new DiagramCache
        {
            Id = Guid.NewGuid(),
            MermaidHash = "hash123",
            RenderedSvg = "<svg>...</svg>",
            Theme = "default",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        cache.Should().NotBeNull();
        cache.Id.Should().NotBe(Guid.Empty);
        cache.MermaidHash.Should().Be("hash123");
        cache.RenderedSvg.Should().Be("<svg>...</svg>");
        cache.Theme.Should().Be("default");
    }

    [Fact]
    public void Create_DefaultValues_AreEmpty()
    {
        var cache = new DiagramCache();

        cache.Id.Should().Be(Guid.Empty);
        cache.MermaidHash.Should().BeEmpty();
        cache.RenderedSvg.Should().BeEmpty();
        cache.Theme.Should().BeEmpty();
    }

    [Theory]
    [InlineData("default")]
    [InlineData("dark")]
    [InlineData("forest")]
    [InlineData("neutral")]
    public void Theme_CanBeSetToAllowedValues(string theme)
    {
        var cache = new DiagramCache { Theme = theme };

        cache.Theme.Should().Be(theme);
    }

    [Fact]
    public void ExpiresAt_CanBeInThePast()
    {
        var cache = new DiagramCache
        {
            ExpiresAt = DateTime.UtcNow.AddHours(-1)
        };

        cache.ExpiresAt.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void RenderedSvg_AllowsLargeContent()
    {
        var largeSvg = new string('a', 100_000);
        var cache = new DiagramCache { RenderedSvg = largeSvg };

        cache.RenderedSvg.Should().HaveLength(100_000);
    }

    [Fact]
    public void MermaidHash_Stores_SHA256Hash()
    {
        var hash = "a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6";
        var cache = new DiagramCache { MermaidHash = hash };

        cache.MermaidHash.Should().Be(hash);
    }
}