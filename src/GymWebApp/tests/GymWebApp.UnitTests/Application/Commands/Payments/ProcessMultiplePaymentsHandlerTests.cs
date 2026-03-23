using FluentAssertions;
using GymWebApp.Application.CQRS.Payments;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.Payments;

public class ProcessMultiplePaymentsHandlerTests
{
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly ProcessMultiplePayments.Handler _handler;

    public ProcessMultiplePaymentsHandlerTests()
    {
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();

        _handler = new ProcessMultiplePayments.Handler(
            _gymMembershipRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _clientRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ProcessesMultiplePayments()
    {
        var command = new ProcessMultiplePayments.Command
        {
            MembershipId = 1,
            PaymentIds = [1, 2],
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            CreatedById = "user-123"
        };

        var client = new Client { Id = 1, AccountId = "user-123" };

        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(2),
            MembershipPlan = new MembershipPlan { Type = "Premium", Price = 49.99m, DurationInMonths = 2 },
            Status = MembershipStatus.Active,
            Payments = new List<Payment>
            {
                new Payment { Id = 1, Status = PaymentStatus.Pending, DueDate = DateTime.UtcNow, Amount = 49.99m },
                new Payment { Id = 2, Status = PaymentStatus.Pending, DueDate = DateTime.UtcNow.AddMonths(1), Amount = 49.99m }
            }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().HaveCount(2);
        result.All(r => r.Success).Should().BeTrue();
        _paymentRepositoryMock.Verify(x => x.Update(It.IsAny<Payment>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ExceedsThreePayments_ReturnsFailure()
    {
        var command = new ProcessMultiplePayments.Command
        {
            MembershipId = 1,
            PaymentIds = [1, 2, 3, 4],
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            CreatedById = "user-123"
        };

        var client = new Client { Id = 1, AccountId = "user-123" };
        var membership = new GymMembership { Id = 1, ClientId = 1, MembershipPlan = new MembershipPlan { Type = "Premium" } };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Success.Should().BeFalse();
        result.First().Message.Should().Contain("up to 3 payments");
    }

    [Fact]
    public async Task Handle_PaymentAlreadyPaid_SkipsAndContinues()
    {
        var command = new ProcessMultiplePayments.Command
        {
            MembershipId = 1,
            PaymentIds = [1, 2],
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            CreatedById = "user-123"
        };

        var client = new Client { Id = 1, AccountId = "user-123" };

        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlan = new MembershipPlan { Type = "Premium" },
            Payments = new List<Payment>
            {
                new Payment { Id = 1, Status = PaymentStatus.Paid, Amount = 49.99m },
                new Payment { Id = 2, Status = PaymentStatus.Pending, Amount = 49.99m }
            }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().HaveCount(2);
        result.First(r => r.PaymentId == 1).Success.Should().BeFalse();
        result.First(r => r.PaymentId == 2).Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_AllPaymentsPaid_ActivatesMembership()
    {
        var command = new ProcessMultiplePayments.Command
        {
            MembershipId = 1,
            PaymentIds = [1],
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            CreatedById = "user-123"
        };

        var client = new Client { Id = 1, AccountId = "user-123" };

        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlan = new MembershipPlan { Type = "Premium" },
            Status = MembershipStatus.Active,
            Payments = new List<Payment>
            {
                new Payment { Id = 1, Status = PaymentStatus.Overdue, DueDate = DateTime.UtcNow.AddDays(-1), Amount = 49.99m }
            }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        await _handler.Handle(command, CancellationToken.None);

        _gymMembershipRepositoryMock.Verify(x => x.Update(It.Is<GymMembership>(m => m.IsActive == true)), Times.Once);
    }
}