using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;
using BusinessRuleViolationException = GymWebApp.Application.Common.Exceptions.BusinessRuleViolationException;

namespace GymWebApp.UnitTests.Application.Commands.RestoreShift;

public class RestoreShiftHandlerTests
{
    private readonly Mock<IShiftRepository> _shiftRepositoryMock;
    private readonly Mock<ITrainerService> _trainerServiceMock;
    private readonly GymWebApp.Application.CQRS.Shifts.RestoreShift.Handler _handler;

    public RestoreShiftHandlerTests()
    {
        _shiftRepositoryMock = new Mock<IShiftRepository>();
        _trainerServiceMock = new Mock<ITrainerService>();
        _handler = new GymWebApp.Application.CQRS.Shifts.RestoreShift.Handler(
            _shiftRepositoryMock.Object,
            _trainerServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_RestoresShift()
    {
        var shift = new Shift
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(8),
            IsCancelled = true
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(shift);

        _shiftRepositoryMock.Setup(x => x.ExistsOverlappingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _trainerServiceMock.Setup(x => x.IsAvailableAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _shiftRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        await _handler.Handle(new RestoreShiftCommand(1, "admin-id"), CancellationToken.None);

        shift.IsCancelled.Should().BeFalse();
        _shiftRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShiftNotFound_ThrowsNotFoundException()
    {
        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Shift?)null);

        var act = () => _handler.Handle(new RestoreShiftCommand(999, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NotCancelled_ThrowsBusinessRuleViolationException()
    {
        var shift = new Shift
        {
            Id = 1,
            IsCancelled = false
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(shift);

        var act = () => _handler.Handle(new RestoreShiftCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>()
            .WithMessage("Shift has not been cancelled");
    }

    [Fact]
    public async Task Handle_PastShift_ThrowsBusinessRuleViolationException()
    {
        var shift = new Shift
        {
            Id = 1,
            StartTime = DateTime.UtcNow.AddDays(-1),
            EndTime = DateTime.UtcNow.AddHours(-1),
            IsCancelled = true
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(shift);

        var act = () => _handler.Handle(new RestoreShiftCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>()
            .WithMessage("Cannot restore shift that is in the past");
    }

    [Fact]
    public async Task Handle_OverlappingShift_ThrowsBusinessRuleViolationException()
    {
        var shift = new Shift
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(8),
            IsCancelled = true
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(shift);

        _shiftRepositoryMock.Setup(x => x.ExistsOverlappingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _handler.Handle(new RestoreShiftCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>()
            .WithMessage("Employee has an overlapping shift at this time");
    }
}
