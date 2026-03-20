using FluentAssertions;
using GymWebApp.Application.CQRS.MembershipPlans;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.MembershipPlans;

public class GetMembershipPlansHandlerTests
{
    private readonly Mock<IMembershipPlanRepository> _membershipPlanRepositoryMock;
    private readonly GetMembershipPlans.Handler _handler;

    public GetMembershipPlansHandlerTests()
    {
        _membershipPlanRepositoryMock = new Mock<IMembershipPlanRepository>();
        _handler = new GetMembershipPlans.Handler(_membershipPlanRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllActivePlans()
    {
        var plans = new List<MembershipPlan>
        {
            new() { Id = 1, Type = "Basic", Price = 29.99m, DurationTime = "30 days", DurationInMonths = 1, IsActive = true },
            new() { Id = 2, Type = "Premium", Price = 99.99m, DurationTime = "30 days", DurationInMonths = 1, IsActive = true }
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(plans);

        var result = await _handler.Handle(new GetMembershipPlansQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[0].Type.Should().Be("Basic");
        result[1].Id.Should().Be(2);
        result[1].Type.Should().Be("Premium");
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoPlans()
    {
        _membershipPlanRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<MembershipPlan>());

        var result = await _handler.Handle(new GetMembershipPlansQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
