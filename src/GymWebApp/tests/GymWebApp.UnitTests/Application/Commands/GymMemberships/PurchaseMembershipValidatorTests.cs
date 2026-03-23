using FluentAssertions;
using GymWebApp.Application.CQRS.GymMemberships;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.GymMemberships;

public class PurchaseMembershipValidatorTests
{
    private readonly PurchaseMembership.Validator _validator;

    public PurchaseMembershipValidatorTests()
    {
        _validator = new PurchaseMembership.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        var command = new PurchaseMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ZeroClientId_Fails()
    {
        var command = new PurchaseMembership.Command
        {
            ClientId = 0,
            MembershipPlanId = 1
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ClientId");
    }

    [Fact]
    public void Validate_NegativeClientId_Fails()
    {
        var command = new PurchaseMembership.Command
        {
            ClientId = -1,
            MembershipPlanId = 1
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ClientId");
    }

    [Fact]
    public void Validate_ZeroMembershipPlanId_Fails()
    {
        var command = new PurchaseMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 0
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MembershipPlanId");
    }

    [Fact]
    public void Validate_NegativeMembershipPlanId_Fails()
    {
        var command = new PurchaseMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = -5
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MembershipPlanId");
    }

    [Fact]
    public void Validate_MissingBothIds_Fails()
    {
        var command = new PurchaseMembership.Command();

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }
}
