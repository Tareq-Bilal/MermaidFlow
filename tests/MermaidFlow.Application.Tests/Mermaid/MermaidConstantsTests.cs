using FluentAssertions;
using MermaidFlow.Application.Mermaid;
using Xunit;

namespace MermaidFlow.Application.Tests.Mermaid;

public class MermaidConstantsTests
{
    [Fact]
    public void MaxCodeLength_HasExpectedValue()
    {
        MermaidConstants.MaxCodeLength.Should().Be(51_200);
    }

    [Fact]
    public void AllowedThemes_ContainsDefaultThemes()
    {
        MermaidConstants.AllowedThemes.Should().Contain("default");
        MermaidConstants.AllowedThemes.Should().Contain("dark");
    }

    [Fact]
    public void AllowedThemes_HasExactlyFourThemes()
    {
        MermaidConstants.AllowedThemes.Should().HaveCount(4);
    }

    [Theory]
    [InlineData("default")]
    [InlineData("dark")]
    [InlineData("forest")]
    [InlineData("neutral")]
    public void AllowedThemes_ContainsTheme(string theme)
    {
        MermaidConstants.AllowedThemes.Should().Contain(theme);
    }

    [Fact]
    public void AllowedThemes_IsReadOnly()
    {
        var themes = MermaidConstants.AllowedThemes;

        themes.Should().NotBeNull();
    }
}