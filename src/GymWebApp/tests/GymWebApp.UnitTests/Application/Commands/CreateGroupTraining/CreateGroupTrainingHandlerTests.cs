using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.CreateGroupTraining;

public class CreateGroupTrainingHandlerTests
{
    private readonly Mock<IGroupTrainingRepository> _groupTrainingRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<ITrainingTypeRepository> _trainingTypeRepositoryMock;
    private readonly Mock<ITrainerService> _trainerServiceMock;
    private readonly GymWebApp.Application.CQRS.GroupTrainings.CreateGroupTraining.Handler _handler;

    public CreateGroupTrainingHandlerTests()
    {
        _groupTrainingRepositoryMock = new Mock<IGroupTrainingRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _trainingTypeRepositoryMock = new Mock<ITrainingTypeRepository>();
        _trainerServiceMock = new Mock<ITrainerService>();
        
        _handler = new GymWebApp.Application.CQRS.GroupTrainings.CreateGroupTraining.Handler(
            _groupTrainingRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _trainingTypeRepositoryMock.Object,
            _trainerServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsTrainingId()
    {
        var command = new CreateGroupTrainingCommand
        {
            TrainerId = 1,
            MaxParticipantNumber = 10,
            TrainingTypeId = 1,
            DifficultyLevel = 2,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            Description = "Test training",
            CreatedById = "admin-id"
        };

        var trainer = new Employee
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Role = EmployeeRole.Trainer,
            Employments = new List<Employment>
            {
                new Employment { Id = 1, StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = null }
            }
        };

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.TrainerId))
            .ReturnsAsync(trainer);

        _trainerServiceMock.Setup(x => x.IsAvailableAsync(command.TrainerId, command.StartDate, command.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _trainingTypeRepositoryMock.Setup(x => x.GetByIdAsync(command.TrainingTypeId))
            .ReturnsAsync(new TrainingType { Id = 1, Name = "Yoga" });

        GroupTraining? capturedTraining = null;
        _groupTrainingRepositoryMock.Setup(x => x.AddAsync(It.IsAny<GroupTraining>()))
            .Callback<GroupTraining>(t => capturedTraining = t);

        _groupTrainingRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Callback(() => { capturedTraining!.Id = 1; })
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(1);
        capturedTraining.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_TrainerNotFound_ThrowsNotFoundException()
    {
        var command = new CreateGroupTrainingCommand
        {
            TrainerId = 999,
            MaxParticipantNumber = 10,
            TrainingTypeId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "admin-id"
        };

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.TrainerId))
            .ReturnsAsync((Employee?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Trainer not found");
    }

    [Fact]
    public async Task Handle_TrainerNotActive_ThrowsValidationException()
    {
        var command = new CreateGroupTrainingCommand
        {
            TrainerId = 1,
            MaxParticipantNumber = 10,
            TrainingTypeId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "admin-id"
        };

        var trainer = new Employee
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Role = EmployeeRole.Trainer,
            Employments = new List<Employment>
            {
                new Employment { Id = 1, StartDate = DateTime.UtcNow.AddMonths(-6), EndDate = DateTime.UtcNow.AddMonths(-1) }
            }
        };

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.TrainerId))
            .ReturnsAsync(trainer);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_TrainerNotAvailable_ThrowsValidationException()
    {
        var command = new CreateGroupTrainingCommand
        {
            TrainerId = 1,
            MaxParticipantNumber = 10,
            TrainingTypeId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "admin-id"
        };

        var trainer = new Employee
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Role = EmployeeRole.Trainer,
            Employments = new List<Employment>
            {
                new Employment { Id = 1, StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = null }
            }
        };

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.TrainerId))
            .ReturnsAsync(trainer);

        _trainerServiceMock.Setup(x => x.IsAvailableAsync(command.TrainerId, command.StartDate, command.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_TrainingTypeNotFound_ThrowsNotFoundException()
    {
        var command = new CreateGroupTrainingCommand
        {
            TrainerId = 1,
            MaxParticipantNumber = 10,
            TrainingTypeId = 999,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "admin-id"
        };

        var trainer = new Employee
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Role = EmployeeRole.Trainer,
            Employments = new List<Employment>
            {
                new Employment { Id = 1, StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = null }
            }
        };

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.TrainerId))
            .ReturnsAsync(trainer);

        _trainerServiceMock.Setup(x => x.IsAvailableAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _trainingTypeRepositoryMock.Setup(x => x.GetByIdAsync(command.TrainingTypeId))
            .ReturnsAsync((TrainingType?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Training type not found");
    }
}
