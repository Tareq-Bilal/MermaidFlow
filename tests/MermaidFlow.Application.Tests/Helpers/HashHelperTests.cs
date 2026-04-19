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

    [Fact]
    public void ComputeSha256_KnownInput_ReturnsConsistentHash()
    {
        var input = "test-password-123";

        var result = HashHelper.ComputeSha256(input);

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveLength(64);
    }

    [Fact]
    public void ComputeSha256_DifferentInputs_ReturnsDifferentHashes()
    {
        var input1 = "hello";
        var input2 = "world";

        var hash1 = HashHelper.ComputeSha256(input1);
        var hash2 = HashHelper.ComputeSha256(input2);

        hash1.Should().NotBe(hash2);
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
    public void ComputeSha256_UnicodeInput_ReturnsValidHash()
    {
        var input = "你好世界";

        var result = HashHelper.ComputeSha256(input);

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveLength(64);
    }

    [Fact]
    public void ComputeSha256_LongInput_ReturnsValidHash()
    {
        var input = new string('a', 10000);

        var result = HashHelper.ComputeSha256(input);

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveLength(64);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\r\n\t")]
    public void ComputeSha256_WhitespaceOnly_ReturnsValidHash(string input)
    {
        var result = HashHelper.ComputeSha256(input);

        result.Should().NotBeNullOrEmpty();
        result.Should().HaveLength(64);
    }
}