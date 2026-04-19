using FluentAssertions;
using MermaidFlow.Domain.Documents;
using Xunit;

namespace MermaidFlow.Application.Tests.Domain;

public class DocumentTests
{
    [Fact]
    public void Create_DefaultConstructor_HasDefaults()
    {
        var doc = new Document();
        doc.Id.Should().Be(Guid.Empty);
        doc.Title.Should().BeEmpty();
        doc.Tags.Should().BeEmpty();
        doc.IsPublic.Should().BeFalse();
    }

    [Fact]
    public void Create_WithProperties_SetsProperties()
    {
        var doc = new Document
        {
            Title = "My Document",
            Content = "# Hello",
            IsPublic = true,
            Tags = new List<string> { "mermaid" }
        };

        doc.Title.Should().Be("My Document");
        doc.IsPublic.Should().BeTrue();
        doc.Tags.Should().Contain("mermaid");
    }

    [Fact]
    public void Tags_AllowsModification()
    {
        var doc = new Document();
        doc.Tags.Add("tag1");
        doc.Tags.Should().Contain("tag1");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IsPublic_CanBeSet(bool isPublic)
    {
        var doc = new Document { IsPublic = isPublic };
        doc.IsPublic.Should().Be(isPublic);
    }
}