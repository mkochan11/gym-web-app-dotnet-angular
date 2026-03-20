using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class UpdateUserValidatorTests
{
    private readonly UpdateUser.Validator _validator;

    public UpdateUserValidatorTests()
    {
        _validator = new UpdateUser.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "123456789",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyId_FailsValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    public void Validate_InvalidEmail_FailsValidation(string email)
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = email,
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_EmptyFirstName_FailsValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "",
            LastName = "Doe",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Validate_FirstNameTooLong_FailsValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = new string('A', 101),
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
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Fact]
    public void Validate_LastNameTooLong_FailsValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = new string('A', 101),
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Fact]
    public void Validate_PhoneNumberTooLong_FailsValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = new string('1', 21),
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PhoneNumber");
    }

    [Fact]
    public void Validate_EmptyPhoneNumber_PassesValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "",
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "PhoneNumber");
    }

    [Fact]
    public void Validate_NullPhoneNumber_PassesValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = null,
            Role = "Client"
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "PhoneNumber");
    }

    [Fact]
    public void Validate_InvalidRole_FailsValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "InvalidRole"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }

    [Fact]
    public void Validate_EmptyRole_FailsValidation()
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = ""
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
    [InlineData("Receptionist")]
    [InlineData("Owner")]
    public void Validate_ValidRole_PassesValidation(string role)
    {
        var command = new UpdateUserCommand
        {
            Id = "user-id-123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = role
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "Role");
    }
}
