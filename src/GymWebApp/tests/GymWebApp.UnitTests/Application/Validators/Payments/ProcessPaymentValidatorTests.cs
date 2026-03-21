using FluentAssertions;
using GymWebApp.Application.CQRS.Payments;
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
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = "John Doe",
            CreatedById = "user-123"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidMembershipPlanId_FailsValidation(int planId)
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = planId,
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = "John Doe"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MembershipPlanId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abc")]
    [InlineData("12345678901234567")]
    public void Validate_InvalidCardNumber_FailsValidation(string cardNumber)
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = cardNumber,
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = "John Doe"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CardNumber");
    }

    [Theory]
    [InlineData("1234567890123456")]
    public void Validate_ValidCardNumber_PassesValidation(string cardNumber)
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = cardNumber,
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = "John Doe"
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "CardNumber");
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("1225")]
    [InlineData("13/25")]
    [InlineData("00/25")]
    public void Validate_InvalidExpiryDate_FailsValidation(string expiryDate)
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "1234567890123456",
            ExpiryDate = expiryDate,
            Cvv = "123",
            CardholderName = "John Doe"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ExpiryDate");
    }

    [Theory]
    [InlineData("12/25")]
    [InlineData("01/30")]
    public void Validate_ValidExpiryDate_PassesValidation(string expiryDate)
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "1234567890123456",
            ExpiryDate = expiryDate,
            Cvv = "123",
            CardholderName = "John Doe"
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "ExpiryDate");
    }

    [Theory]
    [InlineData("")]
    [InlineData("12")]
    [InlineData("12345")]
    [InlineData("abc")]
    public void Validate_InvalidCvv_FailsValidation(string cvv)
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = cvv,
            CardholderName = "John Doe"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Cvv");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("1234")]
    public void Validate_ValidCvv_PassesValidation(string cvv)
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = cvv,
            CardholderName = "John Doe"
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "Cvv");
    }

    [Fact]
    public void Validate_EmptyCardholderName_FailsValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = ""
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CardholderName");
    }

    [Fact]
    public void Validate_LongCardholderName_FailsValidation()
    {
        var longName = new string('A', 201);
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = longName
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CardholderName");
    }

    [Fact]
    public void Validate_ValidCardholderName_PassesValidation()
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = "John Doe"
        };

        var result = _validator.Validate(command);

        result.Errors.Should().NotContain(e => e.PropertyName == "CardholderName");
    }
}