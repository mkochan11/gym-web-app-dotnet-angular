using FluentAssertions;
using GymWebApp.Application.CQRS.GymMemberships;
using GymWebApp.Domain.Enums;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators.GymMemberships;

public class ActivateMembershipValidatorTests
{
    private readonly ActivateMembership.Validator _validator;

    public ActivateMembershipValidatorTests()
    {
        _validator = new ActivateMembership.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_MissingClientId_FailsValidation()
    {
        var command = new ActivateMembership.Command
        {
            ClientId = 0,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ClientId");
    }

    [Fact]
    public void Validate_NegativeClientId_FailsValidation()
    {
        var command = new ActivateMembership.Command
        {
            ClientId = -1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ClientId");
    }

    [Fact]
    public void Validate_MissingMembershipPlanId_FailsValidation()
    {
        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 0,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MembershipPlanId");
    }

    [Theory]
    [InlineData(PaymentMethod.Cash)]
    [InlineData(PaymentMethod.Card)]
    [InlineData(PaymentMethod.BankTransfer)]
    [InlineData(PaymentMethod.Other)]
    public void Validate_ValidPaymentMethod_PassesValidation(PaymentMethod method)
    {
        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = method,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "PaymentMethod");
    }

    [Fact]
    public void Validate_InvalidPaymentMethod_FailsValidation()
    {
        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = (PaymentMethod)999,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PaymentMethod");
    }

    [Fact]
    public void Validate_ZeroAmount_FailsValidation()
    {
        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 0,
            UpdatedById = "staff-1"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Fact]
    public void Validate_NegativeAmount_FailsValidation()
    {
        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = -50.00m,
            UpdatedById = "staff-1"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Fact]
    public void Validate_PositiveAmount_PassesValidation()
    {
        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 1.00m,
            UpdatedById = "staff-1"
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "Amount");
    }

    [Fact]
    public void Validate_AllRequiredFieldsMissing_FailsValidation()
    {
        var command = new ActivateMembership.Command
        {
            ClientId = 0,
            MembershipPlanId = 0,
            PaymentMethod = 0,
            Amount = 0,
            UpdatedById = null
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(3);
    }
}
