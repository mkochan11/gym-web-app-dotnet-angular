using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class CancelIndividualTrainingValidatorTests
{
    private readonly CancelIndividualTraining.Validator _validator;

    public CancelIndividualTrainingValidatorTests()
    {
        _validator = new CancelIndividualTraining.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CancelIndividualTrainingCommand(1, "Sick", "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new CancelIndividualTrainingCommand(0, "Sick", "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptyReason_FailsValidation()
    {
        var command = new CancelIndividualTrainingCommand(1, "", "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
