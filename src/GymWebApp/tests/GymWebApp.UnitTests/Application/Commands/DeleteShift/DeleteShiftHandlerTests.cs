using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.DeleteShift;

public class DeleteShiftHandlerTests
{
    private readonly Mock<IShiftRepository> _shiftRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Shifts.DeleteShift.Handler _handler;

    public DeleteShiftHandlerTests()
    {
        _shiftRepositoryMock = new Mock<IShiftRepository>();
        _handler = new GymWebApp.Application.CQRS.Shifts.DeleteShift.Handler(_shiftRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_CancelledShift_DeletesShift()
    {
        var shift = new Shift
        {
            Id = 1,
            IsCancelled = true,
            CancellationReason = "Sick leave"
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(shift);

        _shiftRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        await _handler.Handle(new DeleteShiftCommand(1, "admin-id"), CancellationToken.None);

        _shiftRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShiftNotFound_ThrowsNotFoundException()
    {
        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Shift?)null);

        var act = () => _handler.Handle(new DeleteShiftCommand(999, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NonCancelledShift_ThrowsValidationException()
    {
        var shift = new Shift
        {
            Id = 1,
            IsCancelled = false
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(shift);

        var act = () => _handler.Handle(new DeleteShiftCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
