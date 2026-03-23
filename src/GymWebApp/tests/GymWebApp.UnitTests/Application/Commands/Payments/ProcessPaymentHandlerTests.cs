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
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly ProcessPayment.Handler _handler;

    public ProcessPaymentHandlerTests()
    {
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();

        _handler = new ProcessPayment.Handler(
            _gymMembershipRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _clientRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesPendingPayment()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            CreatedById = "user-123"
        };

        var client = new Client
        {
            Id = 1,
            AccountId = "user-123",
            Name = "John",
            Surname = "Doe"
        };

        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            Status = MembershipStatus.Active,
            MembershipPlan = new MembershipPlan
            {
                Id = 1,
                Type = "Premium",
                Price = 49.99m,
                DurationInMonths = 1
            },
            Payments = new List<Payment>
            {
                new Payment
                {
                    Id = 1,
                    DueDate = DateTime.UtcNow,
                    Status = PaymentStatus.Pending,
                    Amount = 49.99m
                }
            }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123"))
            .ReturnsAsync(client);

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.MembershipId.Should().Be(1);

        _paymentRepositoryMock.Verify(x => x.Update(It.Is<Payment>(p => p.Status == PaymentStatus.Paid && p.PaymentMethod == PaymentMethod.Card)), Times.Once);
        _paymentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_PaymentIdProvided_UpdatesSpecificPayment()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentId = 2,
            PaymentMethod = PaymentMethod.BankTransfer,
            TransactionId = "BT789012",
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
                new Payment { Id = 1, Status = PaymentStatus.Paid },
                new Payment { Id = 2, Status = PaymentStatus.Pending, Amount = 49.99m }
            }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.PaymentId.Should().Be(2);
    }

    [Fact]
    public async Task Handle_OverduePayment_UpdatesToPaid()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
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
                new Payment
                {
                    Id = 1,
                    Status = PaymentStatus.Overdue,
                    DueDate = DateTime.UtcNow.AddDays(-10),
                    Amount = 49.99m
                }
            }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        _paymentRepositoryMock.Verify(x => x.Update(It.Is<Payment>(p => p.Status == PaymentStatus.Paid)), Times.Once);
    }

    [Fact]
    public async Task Handle_AlreadyPaidPayment_ReturnsFailure()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentId = 1,
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
                new Payment { Id = 1, Status = PaymentStatus.Paid, Amount = 49.99m }
            }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already paid");
    }

    [Fact]
    public async Task Handle_MembershipNotFound_ThrowsNotFoundException()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 999,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            CreatedById = "user-123"
        };

        var client = new Client { Id = 1, AccountId = "user-123" };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(999)).ReturnsAsync((GymMembership?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*GymMembership*999*");
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsNotFoundException()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            CreatedById = "unknown-user"
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("unknown-user")).ReturnsAsync((Client?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*Client*unknown-user*");
    }

    [Fact]
    public async Task Handle_PaymentNotBelongToClient_ThrowsValidationException()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            CreatedById = "user-123"
        };

        var client = new Client { Id = 2, AccountId = "user-123" };

        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlan = new MembershipPlan { Type = "Premium" }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GymWebApp.Application.Common.Exceptions.ValidationException>()
            .WithMessage("*does not belong to this client*");
    }

    [Fact]
    public async Task Handle_NoPendingPayments_ReturnsFailureResult()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
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
            Payments = new List<Payment> { new Payment { Status = PaymentStatus.Cancelled } }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("No pending or overdue payments");
    }

    [Fact]
    public async Task Handle_AllPaymentsPaid_ActivatesMembership()
    {
        var command = new ProcessPayment.Command
        {
            MembershipId = 1,
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
            Status = MembershipStatus.PendingCancellation,
            Payments = new List<Payment>
            {
                new Payment { Id = 1, Status = PaymentStatus.Overdue, DueDate = DateTime.UtcNow.AddDays(-1), Amount = 49.99m }
            }
        };

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);

        await _handler.Handle(command, CancellationToken.None);

        _gymMembershipRepositoryMock.Verify(x => x.Update(It.Is<GymMembership>(m => m.IsActive == true)), Times.Once);
        _gymMembershipRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}