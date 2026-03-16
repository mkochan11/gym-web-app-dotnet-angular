using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class EditGroupTrainingValidatorTests
{
    private readonly EditGroupTraining.Validator _validator;

    public EditGroupTrainingValidatorTests()
    {
        _validator = new EditGroupTraining.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new EditGroupTrainingCommand
        {
            Id = 1,
            TrainerId = 1,
            TrainingTypeId = 1,
            MaxParticipantNumber = 10,
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
        var command = new EditGroupTrainingCommand
        {
            Id = 0,
            TrainerId = 1,
            TrainingTypeId = 1,
            MaxParticipantNumber = 10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            UpdatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
