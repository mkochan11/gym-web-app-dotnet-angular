using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class CreateIndividualTrainingValidatorTests
{
    private readonly CreateIndividualTraining.Validator _validator;

    public CreateIndividualTrainingValidatorTests()
    {
        _validator = new CreateIndividualTraining.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CreateIndividualTrainingCommand
        {
            TrainerId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "client-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidTrainerId_FailsValidation()
    {
        var command = new CreateIndividualTrainingCommand
        {
            TrainerId = 0,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "client-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
