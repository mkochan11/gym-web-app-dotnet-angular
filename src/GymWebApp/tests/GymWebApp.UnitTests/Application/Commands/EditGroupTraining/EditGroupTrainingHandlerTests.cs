using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.UnitTests.Application.Commands.EditGroupTraining;

public class EditGroupTrainingHandlerTests
{
    private readonly Mock<IGroupTrainingRepository> _repositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<ITrainingTypeRepository> _trainingTypeRepositoryMock;
    private readonly Mock<ITrainerService> _trainerServiceMock;
    private readonly GymWebApp.Application.CQRS.GroupTrainings.EditGroupTraining.Handler _handler;

    public EditGroupTrainingHandlerTests()
    {
        _repositoryMock = new Mock<IGroupTrainingRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _trainingTypeRepositoryMock = new Mock<ITrainingTypeRepository>();
        _trainerServiceMock = new Mock<ITrainerService>();
        _handler = new GymWebApp.Application.CQRS.GroupTrainings.EditGroupTraining.Handler(
            _repositoryMock.Object, _employeeRepositoryMock.Object, _trainingTypeRepositoryMock.Object, _trainerServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesTraining()
    {
        var training = new GroupTraining
        {
            Id = 1,
            TrainerId = 1,
            TrainingTypeId = 1,
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(1)
        };

        var command = new EditGroupTrainingCommand
        {
            Id = 1,
            TrainerId = 1,
            TrainingTypeId = 1,
            MaxParticipantNumber = 10,
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(3).AddHours(1),
            UpdatedById = "admin-id"
        };

        var trainer = new Employee { Id = 1, Role = EmployeeRole.Trainer, Employments = new List<Employment> { new() { StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = null } } };
        var trainingType = new TrainingType { Id = 1 };

        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);
        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(1)).ReturnsAsync(trainer);
        _trainingTypeRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(trainingType);
        _trainerServiceMock.Setup(x => x.IsAvailableExcludingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((GroupTraining?)null);

        var act = () => _handler.Handle(new EditGroupTrainingCommand { Id = 999, TrainerId = 1, TrainingTypeId = 1, StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(3).AddHours(1), UpdatedById = "admin" }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_CancelledTraining_ThrowsValidationException()
    {
        var training = new GroupTraining { Id = 1, IsCancelled = true, CancellationReason = "Sick" };
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);

        var act = () => _handler.Handle(new EditGroupTrainingCommand { Id = 1, TrainerId = 1, TrainingTypeId = 1, StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(3).AddHours(1), UpdatedById = "admin" }, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
