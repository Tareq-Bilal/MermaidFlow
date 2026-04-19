using FluentAssertions;
using MermaidFlow.Application.Mermaid;
using MermaidFlow.Application.Mermaid.Commands.RenderMermaid;
using Xunit;

namespace MermaidFlow.Application.Tests.Mermaid;

public class RenderMermaidCommandValidatorTests
{
    private readonly RenderMermaidCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        var result = _validator.Validate(CreateCommand("graph TD\nA --> B", "default"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_EmptyCode_Fails(string? code)
    {
        var result = _validator.Validate(CreateCommand(code!, "default"));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("default")]
    [InlineData("dark")]
    [InlineData("forest")]
    [InlineData("neutral")]
    public void Validate_AllowedThemes_Passes(string theme)
    {
        var result = _validator.Validate(CreateCommand("graph TD\nA --> B", theme));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("light")]
    public void Validate_InvalidTheme_Fails(string theme)
    {
        var result = _validator.Validate(CreateCommand("graph TD\nA --> B", theme));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(MermaidConstants.MaxCodeLength, true)]
    [InlineData(MermaidConstants.MaxCodeLength + 1, false)]
    public void Validate_CodeLength_Passes(int length, bool shouldPass)
    {
        var result = _validator.Validate(CreateCommand(new string('a', length), "default"));
        result.IsValid.Should().Be(shouldPass);
    }

    private static RenderMermaidCommand CreateCommand(string code, string theme) => new(code, theme);
}