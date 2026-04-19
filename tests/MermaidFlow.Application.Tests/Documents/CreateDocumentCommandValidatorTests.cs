using FluentAssertions;
using MermaidFlow.Application.Documents.Commands.CreateDocument;
using Xunit;

namespace MermaidFlow.Application.Tests.Documents;

public class CreateDocumentCommandValidatorTests
{
    private readonly CreateDocumentCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        var command = new CreateDocumentCommand(
            "My Document",
            "# Hello World",
            Guid.NewGuid(),
            true,
            new List<string> { "tag1", "tag2" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NullTitle_Fails()
    {
        var command = new CreateDocumentCommand(
            null!,
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_EmptyTitle_Fails()
    {
        var command = new CreateDocumentCommand(
            "",
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_TitleAtMaximumLength_Passes()
    {
        var command = new CreateDocumentCommand(
            new string('a', 200),
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_TitleExceedsMaximum_Fails()
    {
        var command = new CreateDocumentCommand(
            new string('a', 201),
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyContent_Fails(string? content)
    {
        var command = new CreateDocumentCommand(
            "Title",
            content!,
            Guid.NewGuid(),
            true,
            new List<string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Content");
    }

    [Fact]
    public void Validate_NullTags_Fails()
    {
        var command = new CreateDocumentCommand(
            "Title",
            "# Content",
            Guid.NewGuid(),
            true,
            null!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Tags");
    }

    [Fact]
    public void Validate_EmptyTagsList_Passes()
    {
        var command = new CreateDocumentCommand(
            "Title",
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ValidTags_Passes()
    {
        var command = new CreateDocumentCommand(
            "Title",
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string> { "mermaid", "flowchart", "diagram" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyTagInList_Fails()
    {
        var command = new CreateDocumentCommand(
            "Title",
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string> { "valid", "" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhitespaceTagInList_Fails()
    {
        var command = new CreateDocumentCommand(
            "Title",
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string> { "valid", "   " });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_TagAtMaximumLength_Passes()
    {
        var command = new CreateDocumentCommand(
            "Title",
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string> { new string('a', 50) });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_TagExceedsMaximumLength_Fails()
    {
        var command = new CreateDocumentCommand(
            "Title",
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string> { new string('a', 51) });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Tags[0]");
    }

    [Fact]
    public void Validate_IsPublicTrue_Passes()
    {
        var command = new CreateDocumentCommand(
            "Title",
            "# Content",
            Guid.NewGuid(),
            true,
            new List<string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_IsPublicFalse_Passes()
    {
        var command = new CreateDocumentCommand(
            "Title",
            "# Content",
            Guid.NewGuid(),
            false,
            new List<string>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_MultipleInvalidFields_FailsForAll()
    {
        var command = new CreateDocumentCommand(
            null!,
            "",
            Guid.Empty,
            false,
            null!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().BeGreaterThanOrEqualTo(3);
    }
}