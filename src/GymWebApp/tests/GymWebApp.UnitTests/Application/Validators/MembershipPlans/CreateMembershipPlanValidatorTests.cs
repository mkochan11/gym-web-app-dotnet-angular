using FluentAssertions;
using GymWebApp.Application.CQRS.MembershipPlans;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators.MembershipPlans;

public class CreateMembershipPlanValidatorTests
{
    private readonly CreateMembershipPlan.Validator _validator;

    public CreateMembershipPlanValidatorTests()
    {
        _validator = new CreateMembershipPlan.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CreateMembershipPlanCommand
        {
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
    public void Validate_EmptyType_FailsValidation()
    {
        var command = new CreateMembershipPlanCommand
        {
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
    public void Validate_TypeTooLong_FailsValidation()
    {
        var command = new CreateMembershipPlanCommand
        {
            Type = new string('A', 101),
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidPrice_FailsValidation(decimal price)
    {
        var command = new CreateMembershipPlanCommand
        {
            Type = "Premium",
            Price = price,
            DurationTime = "30 days",
            DurationInMonths = 1
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_EmptyDurationInMonths_FailsValidation()
    {
        var command = new CreateMembershipPlanCommand
        {
            Type = "Premium",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 0
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DurationInMonths");
    }

    [Fact]
    public void Validate_EmptyDurationTime_FailsValidation()
    {
        var command = new CreateMembershipPlanCommand
        {
            Type = "Premium",
            Price = 99.99m,
            DurationTime = "",
            DurationInMonths = 1
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DurationTime");
    }

    [Fact]
    public void Validate_InvalidMaxTrainingsPerMonth_FailsValidation()
    {
        var command = new CreateMembershipPlanCommand
        {
            Type = "Premium",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            MaxTrainingsPerMonth = 0
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MaxTrainingsPerMonth");
    }
}
