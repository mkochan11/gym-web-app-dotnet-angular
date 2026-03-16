using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class CreateGroupTrainingValidatorTests
{
    private readonly CreateGroupTraining.Handler.Validator _validator;

    public CreateGroupTrainingValidatorTests()
    {
        _validator = new CreateGroupTraining.Handler.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CreateGroupTrainingCommand
        {
            TrainerId = 1,
            MaxParticipantNumber = 10,
            TrainingTypeId = 1,
            DifficultyLevel = 2,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            Description = "Test",
            CreatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidMaxParticipants_FailsValidation()
    {
        var command = new CreateGroupTrainingCommand
        {
            TrainerId = 1,
            MaxParticipantNumber = 0,
            TrainingTypeId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MaxParticipantNumber");
    }

    [Fact]
    public void Validate_ShortDuration_FailsValidation()
    {
        var command = new CreateGroupTrainingCommand
        {
            TrainerId = 1,
            MaxParticipantNumber = 10,
            TrainingTypeId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddMinutes(10),
            CreatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
