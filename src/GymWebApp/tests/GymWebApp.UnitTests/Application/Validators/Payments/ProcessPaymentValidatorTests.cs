using FluentAssertions;
using GymWebApp.Application.CQRS.Payments;
using GymWebApp.Domain.Enums;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators.Payments;

public class ProcessPaymentValidatorTests
{
    private readonly ProcessPayment.Validator _validator;

    public ProcessPaymentValidatorTests()
    {
        _validator = new ProcessPayment.Validator();
    }

    [Fact]
    public void Validate_ValidCommand_WithCard_PassesValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            CreatedById = "user-123"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ValidCommand_WithCash_PassesValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.Cash,
            TransactionId = null,
            CreatedById = "user-123"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ValidCommand_WithBankTransfer_PassesValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.BankTransfer,
            TransactionId = "BT789012",
            CreatedById = "user-123"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidMembershipId_FailsValidation(int membershipId)
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = membershipId,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MembershipId");
    }

    [Fact]
    public void Validate_InvalidPaymentMethod_FailsValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = (PaymentMethod)999,
            TransactionId = "TXN123456"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PaymentMethod");
    }

    [Fact]
    public void Validate_MissingTransactionIdForCard_FailsValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = null
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TransactionId");
    }

    [Fact]
    public void Validate_MissingTransactionIdForBankTransfer_FailsValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.BankTransfer,
            TransactionId = null
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TransactionId");
    }

    [Fact]
    public void Validate_MissingTransactionIdForOther_FailsValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.Other,
            TransactionId = null
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TransactionId");
    }

    [Fact]
    public void Validate_TransactionIdTooLong_FailsValidation()
    {
        var longTransactionId = new string('A', 101);
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = longTransactionId
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TransactionId");
    }

    [Fact]
    public void Validate_InvalidPaymentId_FailsValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentId = 0,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PaymentId");
    }

    [Fact]
    public void Validate_ValidPaymentId_PassesValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentId = 5,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456"
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "PaymentId");
    }
}