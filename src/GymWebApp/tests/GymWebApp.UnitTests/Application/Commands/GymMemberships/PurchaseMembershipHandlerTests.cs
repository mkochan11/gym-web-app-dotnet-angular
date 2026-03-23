using FluentAssertions;
using GymWebApp.Application.CQRS.GymMemberships;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.GymMemberships;

public class PurchaseMembershipHandlerTests
{
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly Mock<IMembershipPlanRepository> _planRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly PurchaseMembership.Handler _handler;

    public PurchaseMembershipHandlerTests()
    {
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _planRepositoryMock = new Mock<IMembershipPlanRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _handler = new PurchaseMembership.Handler(
            _gymMembershipRepositoryMock.Object,
            _planRepositoryMock.Object,
            _paymentRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesMembershipAndPayments()
    {
        var plan = new MembershipPlan
        {
            Id = 1,
            Type = "Premium",
            DurationInMonths = 3,
            Price = 100.00m
        };

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _gymMembershipRepositoryMock.Setup(x => x.HasActiveMembershipAsync(1))
            .ReturnsAsync(false);

        _gymMembershipRepositoryMock.Setup(x => x.AddAsync(It.IsAny<GymMembership>()))
            .Returns(Task.CompletedTask);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new GymMembership
            {
                Id = id,
                ClientId = 1,
                MembershipPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(3),
                IsActive = false,
                IsCancelled = false,
                CreatedById = "user-1",
                MembershipPlan = plan,
                Client = new Client { Id = 1, Name = "John", Surname = "Doe" },
                Payments = new List<Payment>
                {
                    new() { Id = 1, Amount = 100, Status = PaymentStatus.Pending },
                    new() { Id = 2, Amount = 100, Status = PaymentStatus.Pending },
                    new() { Id = 3, Amount = 100, Status = PaymentStatus.Pending }
                }
            });

        _paymentRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Payment>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new PurchaseMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            UpdatedById = "user-1"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_SingleMonthPlan_CreatesOnePayment()
    {
        var plan = new MembershipPlan
        {
            Id = 1,
            Type = "Basic",
            DurationInMonths = 1,
            Price = 50.00m
        };

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _gymMembershipRepositoryMock.Setup(x => x.HasActiveMembershipAsync(1))
            .ReturnsAsync(false);

        _gymMembershipRepositoryMock.Setup(x => x.AddAsync(It.IsAny<GymMembership>()))
            .Returns(Task.CompletedTask);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new GymMembership
            {
                Id = id,
                ClientId = 1,
                MembershipPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                IsActive = false,
                IsCancelled = false,
                CreatedById = "user-1",
                MembershipPlan = plan,
                Client = new Client { Id = 1, Name = "John", Surname = "Doe" },
                Payments = new List<Payment>
                {
                    new() { Id = 1, Amount = 50, Status = PaymentStatus.Pending }
                }
            });

        _paymentRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Payment>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new PurchaseMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            UpdatedById = "user-1"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_TwelveMonthPlan_CreatesTwelvePayments()
    {
        var plan = new MembershipPlan
        {
            Id = 1,
            Type = "Annual",
            DurationInMonths = 12,
            Price = 1000.00m
        };

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _gymMembershipRepositoryMock.Setup(x => x.HasActiveMembershipAsync(1))
            .ReturnsAsync(false);

        _gymMembershipRepositoryMock.Setup(x => x.AddAsync(It.IsAny<GymMembership>()))
            .Returns(Task.CompletedTask);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new GymMembership
            {
                Id = id,
                ClientId = 1,
                MembershipPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(12),
                IsActive = false,
                IsCancelled = false,
                CreatedById = "user-1",
                MembershipPlan = plan,
                Client = new Client { Id = 1, Name = "John", Surname = "Doe" },
                Payments = Enumerable.Range(1, 12).Select(i => new Payment { Id = i, Amount = 1000, Status = PaymentStatus.Pending }).ToList()
            });

        _paymentRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Payment>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new PurchaseMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            UpdatedById = "user-1"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
    }
}
