using FluentAssertions;
using GymWebApp.Application.CQRS.MembershipPlans;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.MembershipPlans;

public class GetMembershipPlanByIdHandlerTests
{
    private readonly Mock<IMembershipPlanRepository> _membershipPlanRepositoryMock;
    private readonly GetMembershipPlanById.Handler _handler;

    public GetMembershipPlanByIdHandlerTests()
    {
        _membershipPlanRepositoryMock = new Mock<IMembershipPlanRepository>();
        _handler = new GetMembershipPlanById.Handler(_membershipPlanRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsPlan()
    {
        var plan = new MembershipPlan
        {
            Id = 1,
            Type = "Premium",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            IsActive = true
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        var result = await _handler.Handle(new GetMembershipPlanByIdQuery { Id = 1 }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Type.Should().Be("Premium");
    }

    [Fact]
    public async Task Handle_InvalidId_ReturnsNull()
    {
        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((MembershipPlan?)null);

        var result = await _handler.Handle(new GetMembershipPlanByIdQuery { Id = 999 }, CancellationToken.None);

        result.Should().BeNull();
    }
}
