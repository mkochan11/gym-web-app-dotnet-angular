using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class CreateUserValidatorTests
{
    private readonly CreateUser.Validator _validator;

    public CreateUserValidatorTests()
    {
        _validator = new CreateUser.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Password = "Password123",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    public void Validate_InvalidEmail_FailsValidation(string email)
    {
        var command = new CreateUserCommand
        {
            Email = email,
            Password = "Password123",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567")]
    [InlineData("Pass1")]
    public void Validate_InvalidPassword_FailsValidation(string password)
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Password = password,
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_EmptyFirstName_FailsValidation()
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Password = "Password123",
            FirstName = "",
            LastName = "Doe",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Validate_EmptyLastName_FailsValidation()
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Password = "Password123",
            FirstName = "John",
            LastName = "",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Fact]
    public void Validate_InvalidRole_FailsValidation()
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Password = "Password123",
            FirstName = "John",
            LastName = "Doe",
            Role = "InvalidRole"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }

    [Theory]
    [InlineData("Client")]
    [InlineData("Trainer")]
    [InlineData("Manager")]
    [InlineData("Admin")]
    public void Validate_ValidRole_PassesValidation(string role)
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Password = "Password123",
            FirstName = "John",
            LastName = "Doe",
            Role = role
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "Role");
    }
}
