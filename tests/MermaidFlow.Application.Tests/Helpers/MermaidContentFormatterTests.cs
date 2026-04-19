using FluentAssertions;
using MermaidFlow.Application.Common.Helpers;
using Xunit;

namespace MermaidFlow.Application.Tests.Helpers;

public class MermaidContentFormatterTests
{
    [Fact]
    public void Format_NullInput_ReturnsNull()
    {
        var result = MermaidContentFormatter.Format(null!);
        result.Should().BeNull();
    }

    [Fact]
    public void Format_EmptyString_ReturnsEmpty()
    {
        var result = MermaidContentFormatter.Format(string.Empty);
        result.Should().BeEmpty();
    }

    [Fact]
    public void Format_WhitespaceOnly_ReturnsUnchanged()
    {
        var input = "   ";
        var result = MermaidContentFormatter.Format(input);
        result.Should().Be("   ");
    }

    [Theory]
    [InlineData("test\\nvalue")]
    [InlineData("test\\\\nvalue")]
    [InlineData("\\\"")]
    [InlineData("\\\\\"")]
    public void Format_EscapedSequences_Transformed(string input)
    {
        var result = MermaidContentFormatter.Format(input);
        result.Should().NotBeSameAs(input);
    }

    [Theory]
    [InlineData("```mermaid\ngraph TD\nA --> B\n```")]
    [InlineData("  ```mermaid  \n  graph TD\nA-->B\n  ```  ")]
    public void Format_MarkdownCodeBlock_Removed(string input)
    {
        var result = MermaidContentFormatter.Format(input);
        result.Should().Contain("graph TD");
    }

    [Fact]
    public void Format_NewlinesAtEdges_Trimmed()
    {
        var input = "\n\ngraph TD\nA --> B\n\n";
        var result = MermaidContentFormatter.Format(input);
        result.Should().StartWith("graph TD");
        result.Should().EndWith("A --> B");
    }

    [Fact]
    public void Format_PreservesInternalWhitespace()
    {
        var input = "graph TD\n    A --> B\n    B --> C";
        var result = MermaidContentFormatter.Format(input);
        result.Should().Be(input);
    }
}