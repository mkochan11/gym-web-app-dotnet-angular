using FluentAssertions;
using GymWebApp.Application.CQRS.MembershipPlans;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.MembershipPlans;

public class UpdateMembershipPlanHandlerTests
{
    private readonly Mock<IMembershipPlanRepository> _membershipPlanRepositoryMock;
    private readonly UpdateMembershipPlan.Handler _handler;

    public UpdateMembershipPlanHandlerTests()
    {
        _membershipPlanRepositoryMock = new Mock<IMembershipPlanRepository>();
        _handler = new UpdateMembershipPlan.Handler(_membershipPlanRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedPlan()
    {
        var existingPlan = new MembershipPlan
        {
            Id = 1,
            Type = "Basic",
            Description = "Basic plan",
            Price = 29.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateMembershipPlanCommand
        {
            Id = 1,
            Type = "Basic Plus",
            Description = "Updated basic plan",
            Price = 39.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            CanReserveTrainings = true,
            CanAccessGroupTraining = false,
            CanAccessPersonalTraining = false,
            CanReceiveTrainingPlans = false,
            MaxTrainingsPerMonth = 5,
            IsActive = true
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingPlan);

        _membershipPlanRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<MembershipPlan> { existingPlan });

        _membershipPlanRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Type.Should().Be(command.Type);
        result.Price.Should().Be(command.Price);
        result.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_PlanNotFound_ThrowsNotFoundException()
    {
        var command = new UpdateMembershipPlanCommand
        {
            Id = 999,
            Type = "Premium",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((MembershipPlan?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Entity *MembershipPlan* (999) was not found.");
    }

    [Fact]
    public async Task Handle_DuplicateType_ThrowsValidationException()
    {
        var existingPlan = new MembershipPlan
        {
            Id = 1,
            Type = "Basic",
            Price = 29.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            IsActive = true
        };

        var otherPlan = new MembershipPlan
        {
            Id = 2,
            Type = "Premium",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            IsActive = true
        };

        var command = new UpdateMembershipPlanCommand
        {
            Id = 1,
            Type = "Premium",
            Price = 39.99m,
            DurationTime = "30 days",
            DurationInMonths = 1
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingPlan);

        _membershipPlanRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<MembershipPlan> { existingPlan, otherPlan });

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("A membership plan with this type already exists");
    }
}
