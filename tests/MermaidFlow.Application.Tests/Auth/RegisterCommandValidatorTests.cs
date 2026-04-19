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
        var command = new RegisterCommand("test@example.com", "password123", "John Doe");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NullEmail_Fails()
    {
        var command = new RegisterCommand(null!, "password123", "John");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_NullPassword_Fails()
    {
        var command = new RegisterCommand("test@example.com", null!, "John");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_NullDisplayName_Fails()
    {
        var command = new RegisterCommand("test@example.com", "password123", null!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DisplayName");
    }

    [Fact]
    public void Validate_PasswordTooShort_Fails()
    {
        var command = new RegisterCommand("test@example.com", "short", "John");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_PasswordAtMinimumLength_Passes()
    {
        var command = new RegisterCommand("test@example.com", "12345678", "John");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_PasswordAtMaximumLength_Passes()
    {
        var command = new RegisterCommand("test@example.com", new string('a', 100), "John");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_PasswordExceedsMaximum_Fails()
    {
        var command = new RegisterCommand("test@example.com", new string('a', 101), "John");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_EmailAtMaximumLength_Passes()
    {
        var email = new string('a', 240) + "@domain.com";
        var command = new RegisterCommand(email, "password123", "John");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_DisplayNameAtMaximumLength_Passes()
    {
        var command = new RegisterCommand("test@example.com", "password123", new string('a', 100));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_DisplayNameExceedsMaximum_Fails()
    {
        var command = new RegisterCommand("test@example.com", "password123", new string('a', 101));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DisplayName");
    }

    [Fact]
    public void Validate_EmptyDisplayName_Fails()
    {
        var command = new RegisterCommand("test@example.com", "password123", "");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DisplayName");
    }

    [Fact]
    public void Validate_WhitespaceDisplayName_Fails()
    {
        var command = new RegisterCommand("test@example.com", "password123", "   ");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DisplayName");
    }
}