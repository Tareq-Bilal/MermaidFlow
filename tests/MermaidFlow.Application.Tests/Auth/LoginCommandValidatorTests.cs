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
        var command = new LoginCommand("test@example.com", "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NullEmail_Fails()
    {
        var command = new LoginCommand(null!, "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_EmptyEmail_Fails()
    {
        var command = new LoginCommand("", "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_WhitespaceEmail_Fails()
    {
        var command = new LoginCommand("   ", "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@domain.com")]
    [InlineData("invalid domain.com")]
    public void Validate_InvalidEmailFormat_Fails(string email)
    {
        var command = new LoginCommand(email, "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("test@domain.com")]
    [InlineData("test.name@domain.com")]
    [InlineData("test+tag@domain.com")]
    [InlineData("test@sub.domain.com")]
    [InlineData("a@b.co")]
    [InlineData("test@domain.co.uk")]
    [InlineData("invalid@domain")]
    [InlineData("invalid@domain.")]
    public void Validate_ValidEmailFormats_Passes(string email)
    {
        var command = new LoginCommand(email, "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NullPassword_Fails(string password)
    {
        var command = new LoginCommand("test@example.com", password);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("p")]
    [InlineData("1234567")]
    [InlineData("verylongpasswordbutstillnotverycomplex")]
    public void Validate_ShortPassword_Passes(string password)
    {
        var command = new LoginCommand("test@example.com", password);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("test@domain.com", "    ")]
    public void Validate_MultipleInvalidFields_BothFail(string email, string password)
    {
        var command = new LoginCommand(email, password);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().BeGreaterThan(0);
    }
}