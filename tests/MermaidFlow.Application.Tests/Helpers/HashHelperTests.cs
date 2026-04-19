using FluentAssertions;
using MermaidFlow.Application.Common.Helpers;
using Xunit;

namespace MermaidFlow.Application.Tests.Helpers;

public class HashHelperTests
{
    [Fact]
    public void ComputeSha256_EmptyString_ReturnsValidHash()
    {
        var result = HashHelper.ComputeSha256(string.Empty);
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveLength(64);
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("test-password-123")]
    [InlineData("你好世界")]
    public void ComputeSha256_ValidInput_ReturnsHash(string input)
    {
        var result = HashHelper.ComputeSha256(input);
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveLength(64);
    }

    [Fact]
    public void ComputeSha256_SameInput_ReturnsSameHash()
    {
        var input = "consistent-input";
        var hash1 = HashHelper.ComputeSha256(input);
        var hash2 = HashHelper.ComputeSha256(input);
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ComputeSha256_DifferentInputs_ReturnsDifferentHashes()
    {
        var hash1 = HashHelper.ComputeSha256("hello");
        var hash2 = HashHelper.ComputeSha256("world");
        hash1.Should().NotBe(hash2);
    }
}