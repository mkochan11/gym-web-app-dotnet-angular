using FluentAssertions;
using GymWebApp.Application.CQRS.Payments;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.Payments;

public class GetClientPaymentScheduleHandlerTests
{
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly GetClientPaymentSchedule.Handler _handler;

    public GetClientPaymentScheduleHandlerTests()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _handler = new GetClientPaymentSchedule.Handler(
            _clientRepositoryMock.Object,
            _gymMembershipRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidClient_ReturnsSchedule()
    {
        // Arrange
        var clientId = 1;
        var membershipId = 10;
        var query = new GetClientPaymentSchedule.Query { ClientId = clientId };

        var client = new Client { Id = clientId, Name = "John", Surname = "Doe" };
        var payments = new List<Payment>
        {
            new() { Id = 1, GymMembershipId = membershipId, Status = PaymentStatus.Paid, Amount = 100, DueDate = DateTime.UtcNow.AddDays(-30), Removed = false },
            new() { Id = 2, GymMembershipId = membershipId, Status = PaymentStatus.Pending, Amount = 100, DueDate = DateTime.UtcNow.AddDays(30), Removed = false }
        };
        var membership = new GymMembership
        {
            Id = membershipId,
            ClientId = clientId,
            Status = MembershipStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow.AddDays(330),
            Payments = payments,
            MembershipPlan = new MembershipPlan { Type = "Premium" }
        };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByClientIdWithDetailsAsync(clientId)).ReturnsAsync(membership);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ClientId.Should().Be(clientId);
        result.ClientName.Should().Be("John");
        result.ClientSurname.Should().Be("Doe");
        result.MembershipId.Should().Be(membershipId);
        result.PlanName.Should().Be("Premium");
        result.IsActive.Should().BeTrue();
        result.Payments.Should().HaveCount(2);
        result.TotalPayments.Should().Be(2);
        result.PaidPayments.Should().Be(1);
        result.PendingPayments.Should().Be(1);
        result.OverduePayments.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var query = new GetClientPaymentSchedule.Query { ClientId = 999 };
        _clientRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Client?)null);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NoMembership_ReturnsEmptySchedule()
    {
        // Arrange
        var clientId = 1;
        var query = new GetClientPaymentSchedule.Query { ClientId = clientId };
        var client = new Client { Id = clientId, Name = "John", Surname = "Doe" };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByClientIdWithDetailsAsync(clientId)).ReturnsAsync((GymMembership?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ClientId.Should().Be(clientId);
        result.MembershipId.Should().BeNull();
        result.Payments.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_SoftDeletedPayments_ExcludedFromSchedule()
    {
        // Arrange
        var clientId = 1;
        var membershipId = 10;
        var query = new GetClientPaymentSchedule.Query { ClientId = clientId };

        var client = new Client { Id = clientId, Name = "John", Surname = "Doe" };
        var payments = new List<Payment>
        {
            new() { Id = 1, GymMembershipId = membershipId, Status = PaymentStatus.Paid, Amount = 100, Removed = false },
            new() { Id = 2, GymMembershipId = membershipId, Status = PaymentStatus.Cancelled, Amount = 100, Removed = true }
        };
        var membership = new GymMembership
        {
            Id = membershipId,
            ClientId = clientId,
            Status = MembershipStatus.Active,
            Payments = payments,
            MembershipPlan = new MembershipPlan { Type = "Basic" }
        };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByClientIdWithDetailsAsync(clientId)).ReturnsAsync(membership);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Payments.Should().HaveCount(1);
        result.TotalPayments.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithOverduePayments_CalculatesCorrectly()
    {
        // Arrange
        var clientId = 1;
        var membershipId = 10;
        var query = new GetClientPaymentSchedule.Query { ClientId = clientId };

        var client = new Client { Id = clientId, Name = "John", Surname = "Doe" };
        var payments = new List<Payment>
        {
            new() { Id = 1, GymMembershipId = membershipId, Status = PaymentStatus.Paid, Amount = 100, Removed = false },
            new() { Id = 2, GymMembershipId = membershipId, Status = PaymentStatus.Overdue, Amount = 100, Removed = false },
            new() { Id = 3, GymMembershipId = membershipId, Status = PaymentStatus.Pending, Amount = 100, Removed = false }
        };
        var membership = new GymMembership
        {
            Id = membershipId,
            ClientId = clientId,
            Status = MembershipStatus.Active,
            Payments = payments,
            MembershipPlan = new MembershipPlan { Type = "Premium" }
        };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId)).ReturnsAsync(client);
        _gymMembershipRepositoryMock.Setup(x => x.GetByClientIdWithDetailsAsync(clientId)).ReturnsAsync(membership);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.TotalPayments.Should().Be(3);
        result.PaidPayments.Should().Be(1);
        result.PendingPayments.Should().Be(1);
        result.OverduePayments.Should().Be(1);
    }
}
