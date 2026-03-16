using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class CancelGroupTrainingValidatorTests
{
    private readonly CancelGroupTraining.Validator _validator;

    public CancelGroupTrainingValidatorTests()
    {
        _validator = new CancelGroupTraining.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CancelGroupCommand(1, "Sick", "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new CancelGroupCommand(0, "Sick", "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptyReason_FailsValidation()
    {
        var command = new CancelGroupCommand(1, "", "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
