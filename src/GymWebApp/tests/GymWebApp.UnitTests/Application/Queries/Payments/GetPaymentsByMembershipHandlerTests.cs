using FluentAssertions;
using GymWebApp.Application.CQRS.Payments;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.Payments;

public class GetPaymentsByMembershipHandlerTests
{
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly GetPaymentsByMembership.Handler _handler;

    public GetPaymentsByMembershipHandlerTests()
    {
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();

        _handler = new GetPaymentsByMembership.Handler(
            _gymMembershipRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _clientRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsPayments()
    {
        var query = new GetPaymentsByMembership.Query
        {
            MembershipId = 1,
            UserId = null
        };

        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlan = new MembershipPlan { Type = "Premium" }
        };

        var payments = new List<Payment>
        {
            new Payment { Id = 1, GymMembershipId = 1, DueDate = DateTime.UtcNow, Amount = 49.99m, Status = PaymentStatus.Pending },
            new Payment { Id = 2, GymMembershipId = 1, DueDate = DateTime.UtcNow.AddMonths(1), Amount = 49.99m, Status = PaymentStatus.Paid, PaidDate = DateTime.UtcNow, PaymentMethod = PaymentMethod.Card }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);
        _paymentRepositoryMock.Setup(x => x.GetPaymentsByMembershipIdAsync(1)).ReturnsAsync(payments);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.First().Status.Should().Be("Pending");
        result.Last().Status.Should().Be("Paid");
    }

    [Fact]
    public async Task Handle_MembershipNotFound_ThrowsNotFoundException()
    {
        var query = new GetPaymentsByMembership.Query
        {
            MembershipId = 999,
            UserId = null
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(999)).ReturnsAsync((GymMembership?)null);

        var act = () => _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("*GymMembership*999*");
    }

    [Fact]
    public async Task Handle_UserNotOwner_ThrowsForbiddenException()
    {
        var query = new GetPaymentsByMembership.Query
        {
            MembershipId = 1,
            UserId = "user-123"
        };

        var client = new Client { Id = 2, AccountId = "other-user" };
        var membership = new GymMembership { Id = 1, ClientId = 1 };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);
        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("user-123")).ReturnsAsync(client);

        var act = () => _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsForbiddenException()
    {
        var query = new GetPaymentsByMembership.Query
        {
            MembershipId = 1,
            UserId = "unknown-user"
        };

        var membership = new GymMembership { Id = 1, ClientId = 1 };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);
        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync("unknown-user")).ReturnsAsync((Client?)null);

        var act = () => _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_NoUserId_ReturnsPaymentsWithoutOwnershipCheck()
    {
        var query = new GetPaymentsByMembership.Query
        {
            MembershipId = 1,
            UserId = null
        };

        var membership = new GymMembership { Id = 1, ClientId = 1 };
        var payments = new List<Payment>
        {
            new Payment { Id = 1, GymMembershipId = 1, DueDate = DateTime.UtcNow, Amount = 49.99m, Status = PaymentStatus.Pending }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1)).ReturnsAsync(membership);
        _paymentRepositoryMock.Setup(x => x.GetPaymentsByMembershipIdAsync(1)).ReturnsAsync(payments);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(1);
        _clientRepositoryMock.Verify(x => x.GetByAccountIdAsync(It.IsAny<string>()), Times.Never);
    }
}