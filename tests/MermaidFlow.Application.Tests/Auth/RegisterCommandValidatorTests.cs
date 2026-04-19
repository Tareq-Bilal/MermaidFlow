using FluentAssertions;
using MermaidFlow.Application.Auth.Commands.Register;
using Xunit;

namespace MermaidFlow.Application.Tests.Auth;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        var result = _validator.Validate(new RegisterCommand("test@example.com", "password123", "John"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, "password123", "John", "Email")]
    [InlineData("", "password123", "John", "Email")]
    [InlineData("test@example.com", null, "John", "Password")]
    [InlineData("test@example.com", "", "John", "Password")]
    [InlineData("test@example.com", "password123", null, "DisplayName")]
    [InlineData("test@example.com", "password123", "", "DisplayName")]
    [InlineData("test@example.com", "password123", "   ", "DisplayName")]
    public void Validate_NullOrEmptyFields_Fails(string? email, string? password, string? displayName, string expectedError)
    {
        var result = _validator.Validate(new RegisterCommand(email!, password!, displayName!));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_PasswordTooShort_Fails()
    {
        var result = _validator.Validate(new RegisterCommand("test@example.com", "short", "John"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_PasswordAtMinimum_Passes()
    {
        var result = _validator.Validate(new RegisterCommand("test@example.com", "12345678", "John"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_PasswordAtMaximum_Passes()
    {
        var result = _validator.Validate(new RegisterCommand("test@example.com", new string('a', 100), "John"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_DisplayNameAtMaximum_Passes()
    {
        var result = _validator.Validate(new RegisterCommand("test@example.com", "password123", new string('a', 100)));
        result.IsValid.Should().BeTrue();
    }
}