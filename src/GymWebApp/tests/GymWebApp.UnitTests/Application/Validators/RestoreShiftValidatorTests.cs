using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class RestoreShiftValidatorTests
{
    private readonly RestoreShift.Validator _validator;

    public RestoreShiftValidatorTests()
    {
        _validator = new RestoreShift.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new RestoreShiftCommand(1, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new RestoreShiftCommand(0, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
