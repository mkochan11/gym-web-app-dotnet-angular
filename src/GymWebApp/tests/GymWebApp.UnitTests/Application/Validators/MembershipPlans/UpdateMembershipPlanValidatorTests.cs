using FluentAssertions;
using GymWebApp.Application.CQRS.MembershipPlans;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators.MembershipPlans;

public class UpdateMembershipPlanValidatorTests
{
    private readonly UpdateMembershipPlan.Validator _validator;

    public UpdateMembershipPlanValidatorTests()
    {
        _validator = new UpdateMembershipPlan.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new UpdateMembershipPlanCommand
        {
            Id = 1,
            Type = "Premium",
            Description = "Premium plan description",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            IsActive = true
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_InvalidId_FailsValidation()
    {
        var command = new UpdateMembershipPlanCommand
        {
            Id = 0,
            Type = "Premium",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_EmptyType_FailsValidation()
    {
        var command = new UpdateMembershipPlanCommand
        {
            Id = 1,
            Type = "",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    [Fact]
    public void Validate_NegativePrice_FailsValidation()
    {
        var command = new UpdateMembershipPlanCommand
        {
            Id = 1,
            Type = "Premium",
            Price = -10m,
            DurationTime = "30 days",
            DurationInMonths = 1
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_ZeroDurationInMonths_FailsValidation()
    {
        var command = new UpdateMembershipPlanCommand
        {
            Id = 1,
            Type = "Premium",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 0
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DurationInMonths");
    }
}
