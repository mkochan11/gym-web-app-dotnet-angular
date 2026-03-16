using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class EditShiftValidatorTests
{
    private readonly EditShift.Validator _validator;

    public EditShiftValidatorTests()
    {
        _validator = new EditShift.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new EditShiftCommand
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            UpdatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_FailsValidation(int id)
    {
        var command = new EditShiftCommand
        {
            Id = id,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            UpdatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_StartTimeInPast_FailsValidation()
    {
        var command = new EditShiftCommand
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(-1),
            EndTime = DateTime.UtcNow.AddHours(8),
            UpdatedById = "admin-id"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StartTime");
    }
}
