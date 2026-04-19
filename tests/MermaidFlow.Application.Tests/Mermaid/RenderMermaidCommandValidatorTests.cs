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
        var command = new RenderMermaidCommand("graph TD\nA --> B", "default");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NullCode_Fails()
    {
        var command = new RenderMermaidCommand(null!, "default");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Validate_EmptyCode_Fails()
    {
        var command = new RenderMermaidCommand("", "default");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("default")]
    [InlineData("dark")]
    [InlineData("forest")]
    [InlineData("neutral")]
    public void Validate_AllowedThemes_Passes(string theme)
    {
        var command = new RenderMermaidCommand("graph TD\nA --> B", theme);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_NullOrEmptyTheme_Fails(string theme)
    {
        var command = new RenderMermaidCommand("graph TD\nA --> B", theme);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Theme");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("DEFAULT")]
    [InlineData("Default")]
    [InlineData("light")]
    [InlineData("blue")]
    public void Validate_InvalidTheme_Fails(string theme)
    {
        var command = new RenderMermaidCommand("graph TD\nA --> B", theme);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Theme");
    }

    [Fact]
    public void Validate_CodeAtMaximumLength_Passes()
    {
        var code = new string('a', MermaidConstants.MaxCodeLength);
        var command = new RenderMermaidCommand(code, "default");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_CodeExceedsMaximumLength_Fails()
    {
        var code = new string('a', MermaidConstants.MaxCodeLength + 1);
        var command = new RenderMermaidCommand(code, "default");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Validate_ValidCodeWithMarkdown_Passes()
    {
        var code = """
            ```mermaid
            graph TD
                A --> B
            ```
            """;
        var command = new RenderMermaidCommand(code, "dark");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComplexMermaid_Passes()
    {
        var code = """
            graph TD
                A[Start] --> B{Decision}
                B -->|Yes| C[Process 1]
                B -->|No| D[Process 2]
                C --> E[End]
                D --> E
            """;
        var command = new RenderMermaidCommand(code, "forest");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_MultipleErrors_BothFail()
    {
        var command = new RenderMermaidCommand("", "invalid-theme");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(2);
    }
}