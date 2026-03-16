using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using GymWebApp.Application.DTOs.GroupTraining;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.GetGroupTrainingsFiltered;

public class GetGroupTrainingsFilteredHandlerTests
{
    private readonly Mock<IGroupTrainingRepository> _repositoryMock;
    private readonly GymWebApp.Application.CQRS.GroupTrainings.GetGroupTrainingsFiltered.Handler _handler;

    public GetGroupTrainingsFilteredHandlerTests()
    {
        _repositoryMock = new Mock<IGroupTrainingRepository>();
        _handler = new GymWebApp.Application.CQRS.GroupTrainings.GetGroupTrainingsFiltered.Handler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsFilteredTrainings()
    {
        var trainings = new List<GroupTraining>
        {
            new() { Id = 1, TrainerId = 1, TrainingTypeId = 1, StartTime = DateTime.UtcNow.AddDays(1), EndTime = DateTime.UtcNow.AddDays(1).AddHours(1), TrainingType = new TrainingType { Id = 1, Name = "Yoga" } },
            new() { Id = 2, TrainerId = 1, TrainingTypeId = 2, StartTime = DateTime.UtcNow.AddDays(2), EndTime = DateTime.UtcNow.AddDays(2).AddHours(1), TrainingType = new TrainingType { Id = 2, Name = "HIIT" } }
        };

        _repositoryMock.Setup(x => x.GetFilteredGroupTrainingsAsync(It.IsAny<GroupTrainingFiltersDto>()))
            .ReturnsAsync(trainings);

        var result = await _handler.Handle(new GetGroupTrainingsFilteredQuery(null, null, "1", null), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoTrainings()
    {
        _repositoryMock.Setup(x => x.GetFilteredGroupTrainingsAsync(It.IsAny<GroupTrainingFiltersDto>()))
            .ReturnsAsync(new List<GroupTraining>());

        var result = await _handler.Handle(new GetGroupTrainingsFilteredQuery(null, null, null, null), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
