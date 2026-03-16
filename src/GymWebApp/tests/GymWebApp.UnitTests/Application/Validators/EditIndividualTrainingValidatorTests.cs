using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class EditIndividualTrainingValidatorTests
{
    private readonly EditIndividualTraining.Validator _validator;

    public EditIndividualTrainingValidatorTests()
    {
        _validator = new EditIndividualTraining.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new EditIndividualTrainingCommand
        {
            Id = 1,
            TrainerId = 1,
            ClientId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            UpdatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new EditIndividualTrainingCommand
        {
            Id = 0,
            TrainerId = 1,
            ClientId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            UpdatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
