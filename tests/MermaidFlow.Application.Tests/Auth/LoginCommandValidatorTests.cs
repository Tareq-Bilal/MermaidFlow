using FluentAssertions;
using MermaidFlow.Application.Auth.Commands.Login;
using Xunit;

namespace MermaidFlow.Application.Tests.Auth;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        var result = _validator.Validate(new LoginCommand("test@example.com", "password123"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_InvalidEmail_Fails(string? email)
    {
        var result = _validator.Validate(new LoginCommand(email!, "password123"));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@domain.com")]
    [InlineData("invalid domain.com")]
    public void Validate_InvalidEmailFormat_Fails(string email)
    {
        var result = _validator.Validate(new LoginCommand(email, "password123"));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("test@domain.com")]
    [InlineData("test.name@domain.com")]
    [InlineData("test+tag@domain.com")]
    public void Validate_ValidEmailFormats_Passes(string email)
    {
        var result = _validator.Validate(new LoginCommand(email, "password123"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NullPassword_Fails(string? password)
    {
        var result = _validator.Validate(new LoginCommand("test@example.com", password!));
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("p")]
    [InlineData("1234567")]
    public void Validate_ShortPassword_Passes(string password)
    {
        var result = _validator.Validate(new LoginCommand("test@example.com", password));
        result.IsValid.Should().BeTrue();
    }
}