using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.UnitTests.Application.Commands.EditIndividualTraining;

public class EditIndividualTrainingHandlerTests
{
    private readonly Mock<IIndividualTrainingRepository> _repositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<ITrainerService> _trainerServiceMock;
    private readonly GymWebApp.Application.CQRS.IndividualTrainings.EditIndividualTraining.Handler _handler;

    public EditIndividualTrainingHandlerTests()
    {
        _repositoryMock = new Mock<IIndividualTrainingRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _trainerServiceMock = new Mock<ITrainerService>();
        _handler = new GymWebApp.Application.CQRS.IndividualTrainings.EditIndividualTraining.Handler(
            _repositoryMock.Object, _employeeRepositoryMock.Object, _trainerServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesTraining()
    {
        var training = new IndividualTraining { Id = 1, TrainerId = 1, StartTime = DateTime.UtcNow.AddDays(2), EndTime = DateTime.UtcNow.AddDays(2).AddHours(1) };
        var command = new EditIndividualTrainingCommand { Id = 1, TrainerId = 1, ClientId = 1, StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(3).AddHours(1), UpdatedById = "admin" };
        var trainer = new Employee { Id = 1, Role = EmployeeRole.Trainer, Employments = new List<Employment> { new() { StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = null } } };

        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);
        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(1)).ReturnsAsync(trainer);
        _trainerServiceMock.Setup(x => x.IsAvailableExcludingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((IndividualTraining?)null);

        var act = () => _handler.Handle(new EditIndividualTrainingCommand { Id = 999, TrainerId = 1, ClientId = 1, StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(3).AddHours(1), UpdatedById = "admin" }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_CancelledTraining_ThrowsValidationException()
    {
        var training = new IndividualTraining { Id = 1, IsCancelled = true };
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);

        var act = () => _handler.Handle(new EditIndividualTrainingCommand { Id = 1, TrainerId = 1, ClientId = 1, StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(3).AddHours(1), UpdatedById = "admin" }, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
