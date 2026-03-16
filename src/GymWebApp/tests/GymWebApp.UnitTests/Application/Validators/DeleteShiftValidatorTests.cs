using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators;

public class DeleteShiftValidatorTests
{
    private readonly DeleteShift.Validator _validator;

    public DeleteShiftValidatorTests()
    {
        _validator = new DeleteShift.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new DeleteShiftCommand(1, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new DeleteShiftCommand(0, "admin-id");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
