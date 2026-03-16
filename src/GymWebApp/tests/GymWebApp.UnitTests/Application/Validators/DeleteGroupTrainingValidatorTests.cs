using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class DeleteGroupTrainingValidatorTests
{
    private readonly DeleteGroupTraining.Validator _validator;

    public DeleteGroupTrainingValidatorTests()
    {
        _validator = new DeleteGroupTraining.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new DeleteGroupTrainingCommand(1, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new DeleteGroupTrainingCommand(0, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
