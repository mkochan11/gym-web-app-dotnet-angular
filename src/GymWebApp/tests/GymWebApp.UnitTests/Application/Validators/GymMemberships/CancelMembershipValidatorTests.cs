using FluentAssertions;
using GymWebApp.Application.CQRS.GymMemberships;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators.GymMemberships;

public class CancelMembershipValidatorTests
{
    private readonly CancelMembership.Validator _validator;

    public CancelMembershipValidatorTests()
    {
        _validator = new CancelMembership.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CancelMembership.Command
        {
            MembershipId = 1,
            CancellationReason = "Test reason"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ZeroMembershipId_FailsValidation()
    {
        var command = new CancelMembership.Command
        {
            MembershipId = 0
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MembershipId");
    }

    [Fact]
    public void Validate_NegativeMembershipId_FailsValidation()
    {
        var command = new CancelMembership.Command
        {
            MembershipId = -5
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MembershipId");
    }

    [Fact]
    public void Validate_EmptyCancellationReason_PassesValidation()
    {
        var command = new CancelMembership.Command
        {
            MembershipId = 1,
            CancellationReason = null
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_TooLongCancellationReason_FailsValidation()
    {
        var command = new CancelMembership.Command
        {
            MembershipId = 1,
            CancellationReason = new string('a', 501)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CancellationReason");
    }

    [Fact]
    public void Validate_MaxLengthCancellationReason_PassesValidation()
    {
        var command = new CancelMembership.Command
        {
            MembershipId = 1,
            CancellationReason = new string('a', 500)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain(e => e.PropertyName == "CancellationReason");
    }
}
