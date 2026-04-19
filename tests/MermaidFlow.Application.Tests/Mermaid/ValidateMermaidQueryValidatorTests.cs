using FluentAssertions;
using MermaidFlow.Application.Mermaid;
using MermaidFlow.Application.Mermaid.Queries.ValidateMermaid;
using Xunit;

namespace MermaidFlow.Application.Tests.Mermaid;

public class ValidateMermaidQueryValidatorTests
{
    private readonly ValidateMermaidQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidMermaidCode_Passes()
    {
        var query = new ValidateMermaidQuery("graph TD\nA --> B");

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NullCode_Fails()
    {
        var query = new ValidateMermaidQuery(null!);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Validate_EmptyCode_Fails()
    {
        var query = new ValidateMermaidQuery("");

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhitespaceCode_Fails()
    {
        var query = new ValidateMermaidQuery("   ");

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("flowchart TD\nA[Start] --> B[End]")]
    [InlineData("sequenceDiagram\nAlice->>John: Hello")]
    [InlineData("classDiagram\nClass01 <|-- User")]
    [InlineData("stateDiagram-v2\n[*] --> Initial")]
    [InlineData("gantt\ntitle Sample")]
    [InlineData("pie title Pets\n\"Dogs\": 386")]
    [InlineData("erDiagram\nCUSTOMER ||--o{ ORDER")]
    [InlineData("journey\ntitle My day")]
    public void Validate_ValidMermaidDiagrams_Passes(string code)
    {
        var query = new ValidateMermaidQuery(code);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_CodeAtMaximumLength_Passes()
    {
        var code = new string('a', MermaidConstants.MaxCodeLength);
        var query = new ValidateMermaidQuery(code);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_CodeExceedsMaximumLength_Fails()
    {
        var code = new string('a', MermaidConstants.MaxCodeLength + 1);
        var query = new ValidateMermaidQuery(code);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Validate_VeryLongValidMermaid_Passes()
    {
        var code = string.Join("\n", Enumerable.Repeat("A --> B", 1000));
        var query = new ValidateMermaidQuery(code);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }
}