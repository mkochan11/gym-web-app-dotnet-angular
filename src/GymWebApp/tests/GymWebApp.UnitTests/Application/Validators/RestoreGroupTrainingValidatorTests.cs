using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class RestoreGroupTrainingValidatorTests
{
    private readonly RestoreGroupTraining.Validator _validator;

    public RestoreGroupTrainingValidatorTests()
    {
        _validator = new RestoreGroupTraining.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new RestoreGroupTrainingCommand(1, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new RestoreGroupTrainingCommand(0, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
