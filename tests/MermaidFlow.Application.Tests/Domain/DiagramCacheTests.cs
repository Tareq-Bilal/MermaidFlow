using FluentAssertions;
using MermaidFlow.Domain.Mermaid;
using Xunit;

namespace MermaidFlow.Application.Tests.Domain;

public class DiagramCacheTests
{
    [Fact]
    public void Create_ReturnsCache()
    {
        var cache = new DiagramCache
        {
            Id = Guid.NewGuid(),
            MermaidHash = "hash123",
            RenderedSvg = "<svg>...</svg>",
            Theme = "default"
        };

        cache.Should().NotBeNull();
        cache.MermaidHash.Should().Be("hash123");
        cache.Theme.Should().Be("default");
    }

    [Fact]
    public void Create_DefaultValues_AreEmpty()
    {
        var cache = new DiagramCache();
        cache.Id.Should().Be(Guid.Empty);
        cache.MermaidHash.Should().BeEmpty();
    }

    [Theory]
    [InlineData("default")]
    [InlineData("dark")]
    [InlineData("forest")]
    [InlineData("neutral")]
    public void Theme_SupportsAllowedValues(string theme)
    {
        var cache = new DiagramCache { Theme = theme };
        cache.Theme.Should().Be(theme);
    }
}