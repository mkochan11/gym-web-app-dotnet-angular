using FluentAssertions;
using GymWebApp.Application.CQRS.Payments;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.Payments;

public class ProcessPaymentHandlerTests
{
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly Mock<IMembershipPlanRepository> _membershipPlanRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly ProcessPayment.Handler _handler;

    public ProcessPaymentHandlerTests()
    {
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _membershipPlanRepositoryMock = new Mock<IMembershipPlanRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();

        _handler = new ProcessPayment.Handler(
            _gymMembershipRepositoryMock.Object,
            _membershipPlanRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _clientRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesMembershipAndPayment()
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

        var plan = new MembershipPlan
        {
            Id = 1,
            Type = "Premium",
            Description = "Premium plan",
            Price = 49.99m,
            DurationInMonths = 1
        };

        var client = new Client
        {
            Id = 1,
            AccountId = "user-123",
            Name = "John",
            Surname = "Doe"
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123"))
            .ReturnsAsync(client);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.MembershipId.Should().NotBeNull();
        result.PaymentId.Should().NotBeNull();
        result.PlanName.Should().Be("Premium");
        result.Amount.Should().Be(49.99m);

        _gymMembershipRepositoryMock.Verify(x => x.AddAsync(It.IsAny<GymMembership>()), Times.Once);
        _gymMembershipRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>()), Times.Once);
        _paymentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_PlanNotFound_ThrowsNotFoundException()
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 999,
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = "John Doe",
            CreatedById = "user-123"
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((MembershipPlan?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*MembershipPlan*999*");
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsNotFoundException()
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "1234567890123456",
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = "John Doe",
            CreatedById = "unknown-user"
        };

        var plan = new MembershipPlan
        {
            Id = 1,
            Type = "Premium",
            Price = 49.99m,
            DurationInMonths = 1
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("unknown-user"))
            .ReturnsAsync((Client?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Client*unknown-user*");
    }

    [Fact]
    public async Task Handle_PaymentFails_ReturnsFailureResult()
    {
        var command = new ProcessPayment.Command
        {
            MembershipPlanId = 1,
            CardNumber = "",
            ExpiryDate = "12/25",
            Cvv = "123",
            CardholderName = "John Doe",
            CreatedById = "user-123"
        };

        var plan = new MembershipPlan
        {
            Id = 1,
            Type = "Premium",
            Price = 49.99m,
            DurationInMonths = 1
        };

        var client = new Client
        {
            Id = 1,
            AccountId = "user-123"
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123"))
            .ReturnsAsync(client);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Payment failed");

        _gymMembershipRepositoryMock.Verify(x => x.AddAsync(It.IsAny<GymMembership>()), Times.Never);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SetsCorrectMembershipDates()
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

        var plan = new MembershipPlan
        {
            Id = 1,
            Type = "Basic",
            Price = 29.99m,
            DurationInMonths = 3
        };

        var client = new Client
        {
            Id = 1,
            AccountId = "user-123"
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123"))
            .ReturnsAsync(client);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.StartDate.Should().NotBeNull();
        result.EndDate.Should().NotBeNull();
        var expectedEndDate = result.StartDate!.Value.AddMonths(3);
        result.EndDate.Should().BeCloseTo(expectedEndDate, TimeSpan.FromSeconds(5));
    }
}