using FluentAssertions;
using GymWebApp.Application.CQRS.GymMemberships;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.GymMemberships;

public class ChangeMembershipPlanHandlerTests
{
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly Mock<IMembershipPlanRepository> _membershipPlanRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly ChangeMembershipPlan.Handler _handler;

    public ChangeMembershipPlanHandlerTests()
    {
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _membershipPlanRepositoryMock = new Mock<IMembershipPlanRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _handler = new ChangeMembershipPlan.Handler(
            _gymMembershipRepositoryMock.Object,
            _membershipPlanRepositoryMock.Object,
            _paymentRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUpgrade_ChangesPlanAndRecalculatesPayments()
    {
        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            StartDate = DateTime.UtcNow.AddMonths(-1),
            EndDate = DateTime.UtcNow.AddMonths(1),
            Status = MembershipStatus.Active,
            Client = new Client { Id = 1, Name = "John", Surname = "Doe" },
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Basic", Price = 30m, DurationInMonths = 3 },
            Payments = new List<Payment>
            {
                new Payment { Id = 1, DueDate = DateTime.UtcNow.AddMonths(-1), Status = PaymentStatus.Paid, Amount = 30m },
                new Payment { Id = 2, DueDate = DateTime.UtcNow.AddMonths(1), Status = PaymentStatus.Pending, Amount = 30m }
            }
        };

        var newPlan = new MembershipPlan { Id = 2, Type = "Premium", Price = 50m, DurationInMonths = 3, IsActive = true };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(newPlan);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _paymentRepositoryMock.Setup(x => x.CancelFuturePaymentsAsync(1, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _gymMembershipRepositoryMock.Setup(x => x.Update(It.IsAny<GymMembership>()));

        _paymentRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<Payment>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _paymentRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var callCount = 0;
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<int>()))
            .Returns<int>(id =>
            {
                callCount++;
                if (callCount == 1)
                    return Task.FromResult<GymMembership?>(membership);
                
                var updated = new GymMembership
                {
                    Id = 1,
                    ClientId = 1,
                    MembershipPlanId = 2,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(3),
                    Status = MembershipStatus.Active,
                    Client = new Client { Id = 1, Name = "John", Surname = "Doe" },
                    MembershipPlan = newPlan,
                    Payments = new List<Payment>()
                };
                return Task.FromResult<GymMembership?>(updated);
            });

        var command = new ChangeMembershipPlan.Command
        {
            MembershipId = 1,
            NewPlanId = 2,
            UpdatedById = "user123"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.MembershipPlanId.Should().Be(2);
        result.PlanName.Should().Be("Premium");
        _gymMembershipRepositoryMock.Verify(x => x.Update(membership), Times.Once);
    }

    [Fact]
    public async Task Handle_MembershipNotFound_ThrowsNotFoundException()
    {
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((GymMembership?)null);

        var act = () => _handler.Handle(new ChangeMembershipPlan.Command { MembershipId = 999, NewPlanId = 1 }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NotActiveMembership_ThrowsNotActiveMembershipException()
    {
        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            Status = MembershipStatus.Cancelled,
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Basic" }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        var act = () => _handler.Handle(new ChangeMembershipPlan.Command { MembershipId = 1, NewPlanId = 2 }, CancellationToken.None);

        await act.Should().ThrowAsync<NotActiveMembershipException>()
            .WithMessage("*not active*");
    }

    [Fact]
    public async Task Handle_NewPlanNotFound_ThrowsNotFoundException()
    {
        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            Status = MembershipStatus.Active,
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Basic" }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((MembershipPlan?)null);

        var act = () => _handler.Handle(new ChangeMembershipPlan.Command { MembershipId = 1, NewPlanId = 999 }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*MembershipPlan*");
    }

    [Fact]
    public async Task Handle_InactivePlan_ThrowsBusinessRuleViolationException()
    {
        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            Status = MembershipStatus.Active,
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Basic" }
        };

        var inactivePlan = new MembershipPlan { Id = 2, Type = "Premium", IsActive = false };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(inactivePlan);

        var act = () => _handler.Handle(new ChangeMembershipPlan.Command { MembershipId = 1, NewPlanId = 2 }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>()
            .WithMessage("*inactive*");
    }

    [Fact]
    public async Task Handle_SamePlanSelected_ThrowsBusinessRuleViolationException()
    {
        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            Status = MembershipStatus.Active,
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Basic", IsActive = true }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(membership.MembershipPlan);

        var act = () => _handler.Handle(new ChangeMembershipPlan.Command { MembershipId = 1, NewPlanId = 1 }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>()
            .WithMessage("*different plan*");
    }
}
