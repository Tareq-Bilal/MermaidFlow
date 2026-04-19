using FluentAssertions;
using MermaidFlow.Application.Mermaid;
using Xunit;

namespace MermaidFlow.Application.Tests.Mermaid;

public class MermaidConstantsTests
{
    [Fact]
    public void MaxCodeLength_Is51200() => MermaidConstants.MaxCodeLength.Should().Be(51_200);

    [Fact]
    public void AllowedThemes_HasFourThemes() => MermaidConstants.AllowedThemes.Should().HaveCount(4);

    [Theory]
    [InlineData("default")]
    [InlineData("dark")]
    [InlineData("forest")]
    [InlineData("neutral")]
    public void AllowedThemes_Contains(string theme) => MermaidConstants.AllowedThemes.Should().Contain(theme);
}