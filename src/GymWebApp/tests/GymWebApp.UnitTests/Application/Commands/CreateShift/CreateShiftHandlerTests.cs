using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.CreateShift;

public class CreateShiftHandlerTests
{
    private readonly Mock<IShiftRepository> _shiftRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Shifts.CreateShift.Handler _handler;

    public CreateShiftHandlerTests()
    {
        _shiftRepositoryMock = new Mock<IShiftRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _handler = new GymWebApp.Application.CQRS.Shifts.CreateShift.Handler(
            _shiftRepositoryMock.Object,
            _employeeRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsShiftId()
    {
        var command = new CreateShiftCommand
        {
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            CreatedById = "admin-id"
        };

        var employee = new Employee
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Employments = new List<Employment>
            {
                new Employment { Id = 1, StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = null }
            }
        };

        Shift? capturedShift = null;
        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.EmployeeId))
            .ReturnsAsync(employee);

        _shiftRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Shift>()))
            .Callback<Shift>(s => capturedShift = s);

        _shiftRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Callback(() => { capturedShift!.Id = 1; })
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeGreaterThan(0);
        capturedShift.Should().NotBeNull();
        capturedShift!.EmployeeId.Should().Be(command.EmployeeId);
        _shiftRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_EmployeeNotFound_ThrowsNotFoundException()
    {
        var command = new CreateShiftCommand
        {
            EmployeeId = 999,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            CreatedById = "admin-id"
        };

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.EmployeeId))
            .ReturnsAsync((Employee?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Entity \"Employee\" (999) was not found.");
    }

    [Fact]
    public async Task Handle_InactiveEmployee_ThrowsValidationException()
    {
        var command = new CreateShiftCommand
        {
            EmployeeId = 1,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            CreatedById = "admin-id"
        };

        var employee = new Employee
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Employments = new List<Employment>
            {
                new Employment { Id = 1, StartDate = DateTime.UtcNow.AddMonths(-6), EndDate = DateTime.UtcNow.AddMonths(-1) }
            }
        };

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.EmployeeId))
            .ReturnsAsync(employee);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
