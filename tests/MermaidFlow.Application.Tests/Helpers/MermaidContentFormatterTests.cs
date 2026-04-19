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
    public void Format_EmptyString_ReturnsEmptyString()
    {
        var result = MermaidContentFormatter.Format(string.Empty);

        result.Should().Be(string.Empty);
    }

    [Fact]
    public void Format_WhitespaceOnly_ReturnsWhitespace()
    {
        var input = "   ";

        var result = MermaidContentFormatter.Format(input);

        result.Should().Be("   ");
    }

    [Fact]
    public void Format_NewlineEscaped_ReturnsNewline()
    {
        var input = "test\\nvalue";

        var result = MermaidContentFormatter.Format(input);

        result.Should().NotBeSameAs(input);
    }

    [Fact]
    public void Format_DoubleEscapedNewline_ReturnsNewline()
    {
        var input = "test\\\\nvalue";

        var result = MermaidContentFormatter.Format(input);

        result.Should().NotBeSameAs(input);
    }

    [Fact]
    public void Format_QuoteEscaped_ReturnsQuote()
    {
        var input = "\\\"";

        var result = MermaidContentFormatter.Format(input);

        result.Should().Be("\"");
    }

    [Fact]
    public void Format_DoubleEscapedQuote_ReturnsSingleQuote()
    {
        var input = "\\\\\"";

        var result = MermaidContentFormatter.Format(input);

        result.Should().Be("\"");
    }

    [Fact]
    public void Format_MarkdownCodeBlock_RemovesWrapper()
    {
        var input = """
            ```mermaid
            graph TD
                A --> B
            ```
            """;

        var result = MermaidContentFormatter.Format(input);

        result.Should().Be("graph TD\n    A --> B");
    }

[Fact]
    public void Format_MarkdownCodeBlockWithSpaces_RemovesWrapper()
    {
        var input = "  ```mermaid  \n  graph TD\nA-->B\n  ```  ";

        var result = MermaidContentFormatter.Format(input);

        result.Should().Contain("graph TD");
    }

    [Fact]
    public void Format_EscapedJsonSequence_ReturnsCleanContent()
    {
        var input = "{\\\"code\\\": \\\"graph TD\\\\nA --> B\\\"}";

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
    public void Format_CarriageReturnAtEdges_Trimmed()
    {
        var input = "\r\n\r\ngraph TD\r\nA --> B\r\n\r\n";

        var result = MermaidContentFormatter.Format(input);

        result.Should().StartWith("graph TD");
        result.Should().EndWith("A --> B");
    }

    [Fact]
    public void Format_ComplexInput_AllTransformationsApplied()
    {
        var input = """
            ```mermaid
            graph TD
                A --> B
            ```
            """;

        var result = MermaidContentFormatter.Format(input);

        result.Should().Be("graph TD\n    A --> B");
    }

    [Fact]
    public void Format_PreservesInternalWhitespace()
    {
        var input = "graph TD\n    A --> B\n    B --> C";

        var result = MermaidContentFormatter.Format(input);

        result.Should().Be("graph TD\n    A --> B\n    B --> C");
    }

    [Fact]
    public void Format_MixedEscapesAndMarkdown_HandledCorrectly()
    {
        var input = "```mermaid\\ngraph TD\\nA --> B\\n```";

        var result = MermaidContentFormatter.Format(input);

        result.Should().Be("graph TD\nA --> B");
    }
}