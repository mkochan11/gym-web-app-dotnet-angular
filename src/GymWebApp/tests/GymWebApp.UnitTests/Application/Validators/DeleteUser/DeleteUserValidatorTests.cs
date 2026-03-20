using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class DeleteUserValidatorTests
{
    private readonly DeleteUser.Validator _validator;

    public DeleteUserValidatorTests()
    {
        _validator = new DeleteUser.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new DeleteUserCommand { UserId = "user-id-123" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyId_FailsValidation()
    {
        var command = new DeleteUserCommand { UserId = "" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public void Validate_WhitespaceId_FailsValidation()
    {
        var command = new DeleteUserCommand { UserId = "   " };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }
}
