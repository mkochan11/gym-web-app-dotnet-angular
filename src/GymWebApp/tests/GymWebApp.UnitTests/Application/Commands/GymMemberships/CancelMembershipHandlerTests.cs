using FluentAssertions;
using GymWebApp.Application.CQRS.GymMemberships;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.GymMemberships;

public class CancelMembershipHandlerTests
{
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly CancelMembership.Handler _handler;

    public CancelMembershipHandlerTests()
    {
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _handler = new CancelMembership.Handler(_gymMembershipRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CancelsMembership()
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
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Premium" }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(new CancelMembership.Command { MembershipId = 1 }, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.IsCancelled.Should().BeTrue();
        result.CancelledAt.Should().NotBeNull();
        _gymMembershipRepositoryMock.Verify(x => x.Update(membership), Times.Once);
        _gymMembershipRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_MembershipNotFound_ThrowsNotFoundException()
    {
        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((GymMembership?)null);

        var act = () => _handler.Handle(new CancelMembership.Command { MembershipId = 999 }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_AlreadyCancelledMembership_ThrowsNotActiveMembershipException()
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
            CancelledAt = DateTime.UtcNow.AddMonths(-1),
            Client = new Client { Id = 1, Name = "John", Surname = "Doe" },
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Premium" }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        var act = () => _handler.Handle(new CancelMembership.Command { MembershipId = 1 }, CancellationToken.None);

        await act.Should().ThrowAsync<NotActiveMembershipException>()
            .WithMessage("*already cancelled*");
    }

    [Fact]
    public async Task Handle_ExpiredMembership_ThrowsNotActiveMembershipException()
    {
        var membership = new GymMembership
        {
            Id = 1,
            ClientId = 1,
            MembershipPlanId = 1,
            StartDate = DateTime.UtcNow.AddMonths(-3),
            EndDate = DateTime.UtcNow.AddDays(-10),
            IsActive = false,
            IsCancelled = false,
            Client = new Client { Id = 1, Name = "John", Surname = "Doe" },
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Premium" }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        var act = () => _handler.Handle(new CancelMembership.Command { MembershipId = 1 }, CancellationToken.None);

        await act.Should().ThrowAsync<NotActiveMembershipException>()
            .WithMessage("*expired*");
    }

    [Fact]
    public async Task Handle_WithCancellationReason_RecordsReason()
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
            MembershipPlan = new MembershipPlan { Id = 1, Type = "Premium" }
        };

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(membership);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var command = new CancelMembership.Command
        {
            MembershipId = 1,
            CancellationReason = "Moving to another city"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.CancellationReason.Should().Be("Moving to another city");
        membership.CancellationReason.Should().Be("Moving to another city");
    }
}
