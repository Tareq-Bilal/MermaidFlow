using FluentAssertions;
using MermaidFlow.Domain.Documents;
using Xunit;

namespace MermaidFlow.Application.Tests.Domain;

public class DocumentTests
{
    [Fact]
    public void Create_DefaultConstructor_HasEmptyCollections()
    {
        var document = new Document();

        document.Id.Should().Be(Guid.Empty);
        document.Title.Should().BeEmpty();
        document.Content.Should().BeEmpty();
        document.Tags.Should().BeEmpty();
        document.IsPublic.Should().BeFalse();
    }

    [Fact]
    public void Create_WithProperties_SetsProperties()
    {
        var userId = Guid.NewGuid();
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Title = "My Document",
            Content = "# Hello",
            UserId = userId,
            IsPublic = true,
            Tags = new List<string> { "mermaid", "test" }
        };

        document.Title.Should().Be("My Document");
        document.Content.Should().Be("# Hello");
        document.UserId.Should().Be(userId);
        document.IsPublic.Should().BeTrue();
        document.Tags.Should().Contain(new[] { "mermaid", "test" });
    }

    [Fact]
    public void Tags_ListInitialization_AllowsModification()
    {
        var document = new Document();
        document.Tags.Add("tag1");

        document.Tags.Should().Contain("tag1");
    }

    [Fact]
    public void Document_DefaultUserReference_IsNull()
    {
        var document = new Document();

        document.User.Should().BeNull();
    }

    [Fact]
    public void Document_NullTags_InitializesToEmptyList()
    {
        var document = new Document();

        document.Tags.Should().NotBeNull();
        document.Tags.Should().BeEmpty();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IsPublic_CanBeSet(bool isPublic)
    {
        var document = new Document { IsPublic = isPublic };

        document.IsPublic.Should().Be(isPublic);
    }

    [Fact]
    public void UpdateTimestamp_TrackedViaProperty()
    {
        var document = new Document
        {
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        document.UpdatedAt = DateTime.UtcNow;

        document.UpdatedAt.Should().BeOnOrAfter(DateTime.UtcNow.AddSeconds(-1));
    }
}