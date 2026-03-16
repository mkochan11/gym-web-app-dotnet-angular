using FluentAssertions;
using GymWebApp.Application.CQRS.Shifts;
using GymWebApp.Application.DTOs;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.GetShiftsFiltered;

public class GetShiftsFilteredHandlerTests
{
    private readonly Mock<IShiftRepository> _shiftRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Shifts.GetShiftsFiltered.Handler _handler;

    public GetShiftsFilteredHandlerTests()
    {
        _shiftRepositoryMock = new Mock<IShiftRepository>();
        _handler = new GymWebApp.Application.CQRS.Shifts.GetShiftsFiltered.Handler(_shiftRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsFilteredShifts()
    {
        var shifts = new List<Shift>
        {
            new() { Id = 1, EmployeeId = 1, StartTime = DateTime.UtcNow.AddDays(1), EndTime = DateTime.UtcNow.AddDays(1).AddHours(8) },
            new() { Id = 2, EmployeeId = 1, StartTime = DateTime.UtcNow.AddDays(2), EndTime = DateTime.UtcNow.AddDays(2).AddHours(8) }
        };

        _shiftRepositoryMock.Setup(x => x.GetFilteredShiftsAsync(It.IsAny<ShiftFiltersDto>()))
            .ReturnsAsync(shifts);

        var result = await _handler.Handle(new GetShiftsFilteredQuery(DateTime.UtcNow, DateTime.UtcNow.AddDays(7), "1"), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoShifts()
    {
        _shiftRepositoryMock.Setup(x => x.GetFilteredShiftsAsync(It.IsAny<ShiftFiltersDto>()))
            .ReturnsAsync(new List<Shift>());

        var result = await _handler.Handle(new GetShiftsFilteredQuery(null, null, null), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
