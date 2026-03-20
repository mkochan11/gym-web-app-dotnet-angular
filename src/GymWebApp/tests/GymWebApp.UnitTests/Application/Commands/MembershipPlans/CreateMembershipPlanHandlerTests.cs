using FluentAssertions;
using GymWebApp.Application.CQRS.MembershipPlans;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.MembershipPlans;

public class CreateMembershipPlanHandlerTests
{
    private readonly Mock<IMembershipPlanRepository> _membershipPlanRepositoryMock;
    private readonly CreateMembershipPlan.Handler _handler;

    public CreateMembershipPlanHandlerTests()
    {
        _membershipPlanRepositoryMock = new Mock<IMembershipPlanRepository>();
        _handler = new CreateMembershipPlan.Handler(_membershipPlanRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreatedPlan()
    {
        var command = new CreateMembershipPlanCommand
        {
            Type = "Premium",
            Description = "Premium plan description",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            CanReserveTrainings = true,
            CanAccessGroupTraining = true,
            CanAccessPersonalTraining = false,
            CanReceiveTrainingPlans = true,
            MaxTrainingsPerMonth = 10,
            IsActive = true
        };

        var createdPlan = new MembershipPlan
        {
            Id = 1,
            Type = command.Type,
            Description = command.Description,
            Price = command.Price,
            DurationTime = command.DurationTime,
            DurationInMonths = command.DurationInMonths,
            CanReserveTrainings = command.CanReserveTrainings,
            CanAccessGroupTraining = command.CanAccessGroupTraining,
            CanAccessPersonalTraining = command.CanAccessPersonalTraining,
            CanReceiveTrainingPlans = command.CanReceiveTrainingPlans,
            MaxTrainingsPerMonth = command.MaxTrainingsPerMonth,
            IsActive = command.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<MembershipPlan>());

        _membershipPlanRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Type.Should().Be(command.Type);
        result.Price.Should().Be(command.Price);
        result.DurationInMonths.Should().Be(command.DurationInMonths);
    }

    [Fact]
    public async Task Handle_DuplicateType_ThrowsValidationException()
    {
        var command = new CreateMembershipPlanCommand
        {
            Type = "Premium",
            Description = "Premium plan",
            Price = 99.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            IsActive = true
        };

        var existingPlans = new List<MembershipPlan>
        {
            new MembershipPlan
            {
                Id = 1,
                Type = "Premium",
                IsActive = true,
                Price = 89.99m,
                DurationTime = "30 days",
                DurationInMonths = 1
            }
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(existingPlans);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("A membership plan with this type already exists");
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryCorrectly()
    {
        var command = new CreateMembershipPlanCommand
        {
            Type = "Basic",
            Description = "Basic plan",
            Price = 29.99m,
            DurationTime = "30 days",
            DurationInMonths = 1,
            IsActive = true
        };

        _membershipPlanRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<MembershipPlan>());

        _membershipPlanRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _membershipPlanRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MembershipPlan>()), Times.Once);
        _membershipPlanRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
