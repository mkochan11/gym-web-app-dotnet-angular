using FluentAssertions;
using GymWebApp.Application.CQRS.MembershipPlans;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.MembershipPlans;

public class DeleteMembershipPlanHandlerTests
{
    private readonly Mock<IMembershipPlanRepository> _membershipPlanRepositoryMock;
    private readonly DeleteMembershipPlan.Handler _handler;

    public DeleteMembershipPlanHandlerTests()
    {
        _membershipPlanRepositoryMock = new Mock<IMembershipPlanRepository>();
        _handler = new DeleteMembershipPlan.Handler(_membershipPlanRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_SoftDeletesPlan()
    {
        var existingPlan = new MembershipPlan
        {
            Id = 1,
            Type = "Premium",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            IsActive = true
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingPlan);

        _membershipPlanRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteMembershipPlanCommand { Id = 1 }, CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        _membershipPlanRepositoryMock.Verify(x => x.Remove(existingPlan), Times.Once);
        _membershipPlanRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
