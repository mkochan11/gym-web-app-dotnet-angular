using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class DeleteIndividualTrainingValidatorTests
{
    private readonly DeleteIndividualTraining.Validator _validator;

    public DeleteIndividualTrainingValidatorTests()
    {
        _validator = new DeleteIndividualTraining.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new DeleteIndividualTrainingCommand(1, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new DeleteIndividualTrainingCommand(0, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
