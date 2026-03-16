using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class RestoreIndividualTrainingValidatorTests
{
    private readonly RestoreIndividualTraining.Validator _validator;

    public RestoreIndividualTrainingValidatorTests()
    {
        _validator = new RestoreIndividualTraining.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new RestoreIndividualTrainingCommand(1, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new RestoreIndividualTrainingCommand(0, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
