using FluentAssertions;
using GymWebApp.Application.CQRS.Payments;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Payment;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.Payments;

public class AcceptPaymentHandlerTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly AcceptPayment.Handler _handler;

    public AcceptPaymentHandlerTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _handler = new AcceptPayment.Handler(
            _paymentRepositoryMock.Object,
            _gymMembershipRepositoryMock.Object,
            _clientRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCashPayment_ReturnsSuccess()
    {
        // Arrange
        var clientId = 1;
        var paymentId = 100;
        var membershipId = 10;
        var command = new AcceptPayment.Command
        {
            ClientId = clientId,
            PaymentId = paymentId,
            PaymentMethod = PaymentMethod.Cash,
            TransactionId = null,
            ProcessedById = "receptionist-id"
        };

        var client = new Client { Id = clientId, Name = "John", Surname = "Doe" };
        var payment = new Payment { Id = paymentId, GymMembershipId = membershipId, Status = PaymentStatus.Pending, Amount = 100 };
        var membership = new GymMembership
        {
            Id = membershipId,
            ClientId = clientId,
            Status = MembershipStatus.Active,
            Payments = new List<Payment> { payment },
            MembershipPlan = new MembershipPlan { Type = "Premium" }
        };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByClientIdWithDetailsAsync(clientId)).ReturnsAsync(membership);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _paymentRepositoryMock.Verify(x => x.Update(It.Is<Payment>(p => p.Status == PaymentStatus.Paid)), Times.Once);
        _paymentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCardPayment_ReturnsSuccess()
    {
        // Arrange
        var clientId = 1;
        var paymentId = 100;
        var membershipId = 10;
        var command = new AcceptPayment.Command
        {
            ClientId = clientId,
            PaymentId = paymentId,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            ProcessedById = "receptionist-id"
        };

        var client = new Client { Id = clientId, Name = "John", Surname = "Doe" };
        var payment = new Payment { Id = paymentId, GymMembershipId = membershipId, Status = PaymentStatus.Pending, Amount = 100 };
        var membership = new GymMembership
        {
            Id = membershipId,
            ClientId = clientId,
            Status = MembershipStatus.Active,
            Payments = new List<Payment> { payment },
            MembershipPlan = new MembershipPlan { Type = "Premium" }
        };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByClientIdWithDetailsAsync(clientId)).ReturnsAsync(membership);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.PaymentMethod.Should().Be("Card");
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new AcceptPayment.Command { ClientId = 999, PaymentId = 100 };
        _clientRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Client?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NoMembership_ReturnsFailure()
    {
        // Arrange
        var clientId = 1;
        var command = new AcceptPayment.Command { ClientId = clientId, PaymentId = 100 };
        var client = new Client { Id = clientId, Name = "John", Surname = "Doe" };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByClientIdWithDetailsAsync(clientId)).ReturnsAsync((GymMembership?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("No active membership");
    }

    [Fact]
    public async Task Handle_PaymentAlreadyPaid_ReturnsFailure()
    {
        // Arrange
        var clientId = 1;
        var paymentId = 100;
        var membershipId = 10;
        var command = new AcceptPayment.Command { ClientId = clientId, PaymentId = paymentId };

        var client = new Client { Id = clientId, Name = "John", Surname = "Doe" };
        var payment = new Payment { Id = paymentId, GymMembershipId = membershipId, Status = PaymentStatus.Paid };
        var membership = new GymMembership
        {
            Id = membershipId,
            ClientId = clientId,
            Payments = new List<Payment> { payment }
        };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByClientIdWithDetailsAsync(clientId)).ReturnsAsync(membership);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already paid");
    }

    [Fact]
    public async Task Handle_OverduePayment_ActivatesMembershipWhenAllPaid()
    {
        // Arrange
        var clientId = 1;
        var paymentId = 100;
        var membershipId = 10;
        var command = new AcceptPayment.Command
        {
            ClientId = clientId,
            PaymentId = paymentId,
            PaymentMethod = PaymentMethod.Cash
        };

        var client = new Client { Id = clientId, Name = "John", Surname = "Doe" };
        var overduePayment = new Payment { Id = paymentId, GymMembershipId = membershipId, Status = PaymentStatus.Overdue, Amount = 100, DueDate = DateTime.UtcNow.AddDays(-5) };
        var membership = new GymMembership
        {
            Id = membershipId,
            ClientId = clientId,
            Status = MembershipStatus.Active,
            Payments = new List<Payment> { overduePayment },
            MembershipPlan = new MembershipPlan { Type = "Premium" }
        };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByClientIdWithDetailsAsync(clientId)).ReturnsAsync(membership);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        _gymMembershipRepositoryMock.Verify(x => x.Update(It.Is<GymMembership>(m => m.Status == MembershipStatus.Active)), Times.Once);
    }
}
