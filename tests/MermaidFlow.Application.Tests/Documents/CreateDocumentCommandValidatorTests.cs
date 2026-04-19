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
        var result = _validator.Validate(CreateCommand("Title", "Content"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_EmptyTitle_Fails(string? title)
    {
        var result = _validator.Validate(CreateCommand(title!, "Content"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Theory]
    [InlineData(200, true)]
    [InlineData(201, false)]
    public void Validate_TitleLength_Passes(int length, bool shouldPass)
    {
        var result = _validator.Validate(CreateCommand(new string('a', length), "Content"));
        result.IsValid.Should().Be(shouldPass);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyContent_Fails(string? content)
    {
        var result = _validator.Validate(CreateCommand("Title", content!));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    public void Validate_NullTags_Passes(List<string>? tags)
    {
        var result = _validator.Validate(CreateCommand("Title", "Content", tags: tags));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Validate_IsPublic_Passes(bool isPublic)
    {
        var result = _validator.Validate(CreateCommand("Title", "Content", isPublic: isPublic));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("valid", true)]
    public void Validate_TagValues(string tag, bool shouldPass)
    {
        var result = _validator.Validate(CreateCommand("Title", "Content", tags: new List<string> { tag }));
        result.IsValid.Should().Be(shouldPass);
    }

    [Theory]
    [InlineData(50, true)]
    [InlineData(51, false)]
    public void Validate_TagLength_Passes(int length, bool shouldPass)
    {
        var result = _validator.Validate(CreateCommand("Title", "Content", tags: new List<string> { new string('a', length) }));
        result.IsValid.Should().Be(shouldPass);
    }

    private static CreateDocumentCommand CreateCommand(
        string title,
        string content,
        bool isPublic = true,
        List<string>? tags = null) => new(title, content, Guid.NewGuid(), isPublic, tags ?? new List<string>());
}