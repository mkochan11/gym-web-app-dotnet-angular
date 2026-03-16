using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.EditShift;

public class EditShiftHandlerTests
{
    private readonly Mock<IShiftRepository> _shiftRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Shifts.EditShift.Handler _handler;

    public EditShiftHandlerTests()
    {
        _shiftRepositoryMock = new Mock<IShiftRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _handler = new GymWebApp.Application.CQRS.Shifts.EditShift.Handler(
            _shiftRepositoryMock.Object,
            _employeeRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesShift()
    {
        var shift = new Shift
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(8)
        };

        var command = new EditShiftCommand
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(3),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(8),
            UpdatedById = "admin-id"
        };

        var employee = new Employee
        {
            Id = 1,
            Name = "John",
            Employments = new List<Employment>
            {
                new Employment { StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = null }
            }
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(shift);

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.EmployeeId))
            .ReturnsAsync(employee);

        _shiftRepositoryMock.Setup(x => x.ExistsOverlappingExcludingAsync(
            command.EmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _shiftRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _shiftRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShiftNotFound_ThrowsNotFoundException()
    {
        var command = new EditShiftCommand
        {
            Id = 999,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(3),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(8),
            UpdatedById = "admin-id"
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync((Shift?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_CancelledShift_ThrowsValidationException()
    {
        var shift = new Shift
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(8),
            IsCancelled = true,
            CancellationReason = "Sick leave"
        };

        var command = new EditShiftCommand
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(3),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(8),
            UpdatedById = "admin-id"
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(shift);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_EmployeeNotActive_ThrowsValidationException()
    {
        var shift = new Shift
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(8)
        };

        var command = new EditShiftCommand
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(3),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(8),
            UpdatedById = "admin-id"
        };

        var employee = new Employee
        {
            Id = 1,
            Name = "John",
            Employments = new List<Employment>
            {
                new Employment { StartDate = DateTime.UtcNow.AddMonths(-6), EndDate = DateTime.UtcNow.AddMonths(-1) }
            }
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(shift);

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.EmployeeId))
            .ReturnsAsync(employee);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_OverlappingShift_ThrowsValidationException()
    {
        var shift = new Shift
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(8)
        };

        var command = new EditShiftCommand
        {
            Id = 1,
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(3),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(8),
            UpdatedById = "admin-id"
        };

        var employee = new Employee
        {
            Id = 1,
            Name = "John",
            Employments = new List<Employment>
            {
                new Employment { StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = null }
            }
        };

        _shiftRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(shift);

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.EmployeeId))
            .ReturnsAsync(employee);

        _shiftRepositoryMock.Setup(x => x.ExistsOverlappingExcludingAsync(
            command.EmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
