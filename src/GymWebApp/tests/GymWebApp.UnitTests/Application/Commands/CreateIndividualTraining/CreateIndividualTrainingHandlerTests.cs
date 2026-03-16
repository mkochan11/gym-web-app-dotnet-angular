using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.CreateIndividualTraining;

public class CreateIndividualTrainingHandlerTests
{
    private readonly Mock<IIndividualTrainingRepository> _trainingRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<ITrainerService> _trainerServiceMock;
    private readonly GymWebApp.Application.CQRS.IndividualTrainings.CreateIndividualTraining.Handler _handler;

    public CreateIndividualTrainingHandlerTests()
    {
        _trainingRepositoryMock = new Mock<IIndividualTrainingRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _trainerServiceMock = new Mock<ITrainerService>();
        
        _handler = new GymWebApp.Application.CQRS.IndividualTrainings.CreateIndividualTraining.Handler(
            _trainingRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _trainerServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsTrainingId()
    {
        var command = new CreateIndividualTrainingCommand
        {
            TrainerId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            Description = "Personal training",
            CreatedById = "client-id"
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

        IndividualTraining? capturedTraining = null;
        _trainingRepositoryMock.Setup(x => x.AddAsync(It.IsAny<IndividualTraining>()))
            .Callback<IndividualTraining>(t => capturedTraining = t);

        _trainingRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Callback(() => { capturedTraining!.Id = 1; })
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(1);
        capturedTraining.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_TrainerNotFound_ThrowsNotFoundException()
    {
        var command = new CreateIndividualTrainingCommand
        {
            TrainerId = 999,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "client-id"
        };

        _employeeRepositoryMock.Setup(x => x.GetByIdWithEmploymentsAsync(command.TrainerId))
            .ReturnsAsync((Employee?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Entity \"Trainer\" (999) was not found.");
    }

    [Fact]
    public async Task Handle_TrainerNotActive_ThrowsValidationException()
    {
        var command = new CreateIndividualTrainingCommand
        {
            TrainerId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "client-id"
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
        var command = new CreateIndividualTrainingCommand
        {
            TrainerId = 1,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(1).AddHours(1),
            CreatedById = "client-id"
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
}
