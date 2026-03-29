using FluentAssertions;
using GymWebApp.Application.CQRS.Payments;
using GymWebApp.Domain.Enums;
using Xunit;

namespace GymWebApp.UnitTests.Application.Validators.Payments;

public class AcceptPaymentValidatorTests
{
    private readonly AcceptPayment.Validator _validator;

    public AcceptPaymentValidatorTests()
    {
        _validator = new AcceptPayment.Validator();
    }

    [Fact]
    public void Validate_ValidCashPayment_PassesValidation()
    {
        // Arrange
        var command = new AcceptPayment.Command
        {
            ClientId = 1,
            PaymentId = 100,
            PaymentMethod = PaymentMethod.Cash,
            TransactionId = null
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ValidCardPayment_PassesValidation()
    {
        // Arrange
        var command = new AcceptPayment.Command
        {
            ClientId = 1,
            PaymentId = 100,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_MissingTransactionIdForCard_FailsValidation()
    {
        // Arrange
        var command = new AcceptPayment.Command
        {
            ClientId = 1,
            PaymentId = 100,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = null
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TransactionId");
    }

    [Fact]
    public void Validate_MissingTransactionIdForBankTransfer_FailsValidation()
    {
        // Arrange
        var command = new AcceptPayment.Command
        {
            ClientId = 1,
            PaymentId = 100,
            PaymentMethod = PaymentMethod.BankTransfer,
            TransactionId = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TransactionId");
    }

    [Fact]
    public void Validate_MissingTransactionIdForOther_FailsValidation()
    {
        // Arrange
        var command = new AcceptPayment.Command
        {
            ClientId = 1,
            PaymentId = 100,
            PaymentMethod = PaymentMethod.Other,
            TransactionId = null
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TransactionId");
    }

    [Fact]
    public void Validate_InvalidClientId_FailsValidation()
    {
        // Arrange
        var command = new AcceptPayment.Command
        {
            ClientId = 0,
            PaymentId = 100,
            PaymentMethod = PaymentMethod.Cash
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ClientId");
    }

    [Fact]
    public void Validate_InvalidPaymentId_FailsValidation()
    {
        // Arrange
        var command = new AcceptPayment.Command
        {
            ClientId = 1,
            PaymentId = 0,
            PaymentMethod = PaymentMethod.Cash
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PaymentId");
    }

    [Fact]
    public void Validate_TransactionIdTooLong_FailsValidation()
    {
        // Arrange
        var command = new AcceptPayment.Command
        {
            ClientId = 1,
            PaymentId = 100,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = new string('x', 101)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TransactionId");
    }
}
