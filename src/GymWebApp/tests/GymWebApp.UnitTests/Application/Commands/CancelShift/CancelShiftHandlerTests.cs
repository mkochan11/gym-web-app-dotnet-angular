using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.UnitTests.Application.Commands.CancelShift;

public class CancelShiftHandlerTests
{
    private readonly Mock<IShiftRepository> _shiftRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Shifts.CancelShift.Handler _handler;

    public CancelShiftHandlerTests()
    {
        _shiftRepositoryMock = new Mock<IShiftRepository>();
        _handler = new GymWebApp.Application.CQRS.Shifts.CancelShift.Handler(_shiftRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CancelsShift()
    {
        var shift = new Shift
        {
            Id = 1,
            IsCancelled = false
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(shift);

        _shiftRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        await _handler.Handle(new CancelShiftCommand(1, "Sick leave", "admin-id"), CancellationToken.None);

        shift.IsCancelled.Should().BeTrue();
        _shiftRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShiftNotFound_ThrowsNotFoundException()
    {
        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Shift?)null);

        var act = () => _handler.Handle(new CancelShiftCommand(999, "Sick leave", "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_AlreadyCancelled_ThrowsValidationException()
    {
        var shift = new Shift
        {
            Id = 1,
            IsCancelled = true
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(shift);

        var act = () => _handler.Handle(new CancelShiftCommand(1, "Sick leave", "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
