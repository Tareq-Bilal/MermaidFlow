using FluentAssertions;
using MermaidFlow.Application.Mermaid;
using MermaidFlow.Application.Mermaid.Queries.ValidateMermaid;
using Xunit;

namespace MermaidFlow.Application.Tests.Mermaid;

public class ValidateMermaidQueryValidatorTests
{
    private readonly ValidateMermaidQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidCode_Passes()
    {
        var result = _validator.Validate(new ValidateMermaidQuery("graph TD\nA --> B"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyCode_Fails(string? code)
    {
        var result = _validator.Validate(new ValidateMermaidQuery(code!));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("flowchart TD\nA --> B")]
    [InlineData("sequenceDiagram\nA->>B: Hi")]
    [InlineData("classDiagram\nA <|-- B")]
    public void Validate_ValidMermaidDiagrams_Passes(string code)
    {
        var result = _validator.Validate(new ValidateMermaidQuery(code));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(MermaidConstants.MaxCodeLength, true)]
    [InlineData(MermaidConstants.MaxCodeLength + 1, false)]
    public void Validate_CodeLength_Passes(int length, bool shouldPass)
    {
        var result = _validator.Validate(new ValidateMermaidQuery(new string('a', length)));
        result.IsValid.Should().Be(shouldPass);
    }
}