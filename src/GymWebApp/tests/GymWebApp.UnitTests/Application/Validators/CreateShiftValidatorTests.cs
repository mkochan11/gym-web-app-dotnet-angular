using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class CreateShiftValidatorTests
{
    private readonly CreateShift.Validator _validator;

    public CreateShiftValidatorTests()
    {
        _validator = new CreateShift.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CreateShiftCommand
        {
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            CreatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidEmployeeId_FailsValidation(int employeeId)
    {
        var command = new CreateShiftCommand
        {
            EmployeeId = employeeId,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            CreatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EmployeeId");
    }

    [Fact]
    public void Validate_StartTimeInPast_FailsValidation()
    {
        var command = new CreateShiftCommand
        {
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(-1),
            EndTime = DateTime.UtcNow.AddHours(8),
            CreatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StartTime");
    }

    [Fact]
    public void Validate_EndTimeBeforeStartTime_FailsValidation()
    {
        var startTime = DateTime.UtcNow.AddDays(1).AddHours(10);
        var command = new CreateShiftCommand
        {
            EmployeeId = 1,
            StartTime = startTime,
            EndTime = startTime.AddHours(-1),
            CreatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EndTime");
    }

    [Fact]
    public void Validate_EmptyCreatedById_FailsValidation()
    {
        var command = new CreateShiftCommand
        {
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            CreatedById = ""
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CreatedById");
    }
}
