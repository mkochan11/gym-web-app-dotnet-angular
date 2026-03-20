using FluentAssertions;
using GymWebApp.Application.CQRS.GymMemberships;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.GymMemberships;

public class GetActiveMembershipHandlerTests
{
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly GetActiveMembership.Handler _handler;

    public GetActiveMembershipHandlerTests()
    {
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _handler = new GetActiveMembership.Handler(_gymMembershipRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ActiveMembershipExists_ReturnsMembership()
    {
        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            StartDate = DateTime.UtcNow.AddMonths(-1),
            EndDate = DateTime.UtcNow.AddMonths(1),
            IsActive = true,
            IsCancelled = false,
            Client = new Client { Id = 1, Name = "John", Surname = "Doe" },
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Premium", Description = "Best plan" }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetActiveMembershipByClientIdAsync(1))
            .ReturnsAsync(membership);

        var result = await _handler.Handle(new GetActiveMembership.Query { ClientId = 1 }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.ClientId.Should().Be(1);
        result.PlanName.Should().Be("Premium");
        result.IsCancelled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NoActiveMembership_ReturnsNull()
    {
        _gymMembershipRepositoryMock.Setup(x => x.GetActiveMembershipByClientIdAsync(999))
            .ReturnsAsync((GymMembership?)null);

        var result = await _handler.Handle(new GetActiveMembership.Query { ClientId = 999 }, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CancelledMembership_ReturnsNull()
    {
        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            StartDate = DateTime.UtcNow.AddMonths(-2),
            EndDate = DateTime.UtcNow.AddMonths(-1),
            IsActive = false,
            IsCancelled = true,
            CancelledAt = DateTime.UtcNow.AddDays(-1),
            Client = new Client { Id = 1, Name = "John", Surname = "Doe" },
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Premium" }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetActiveMembershipByClientIdAsync(1))
            .ReturnsAsync((GymMembership?)null);

        var result = await _handler.Handle(new GetActiveMembership.Query { ClientId = 1 }, CancellationToken.None);

        result.Should().BeNull();
    }
}
