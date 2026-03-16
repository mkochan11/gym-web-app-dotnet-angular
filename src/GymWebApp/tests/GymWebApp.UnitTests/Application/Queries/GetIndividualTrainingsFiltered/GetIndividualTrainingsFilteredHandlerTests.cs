using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using GymWebApp.Application.DTOs;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.GetIndividualTrainingsFiltered;

public class GetIndividualTrainingsFilteredHandlerTests
{
    private readonly Mock<IIndividualTrainingRepository> _repositoryMock;
    private readonly GymWebApp.Application.CQRS.IndividualTrainings.GetIndividualTrainingsFiltered.Handler _handler;

    public GetIndividualTrainingsFilteredHandlerTests()
    {
        _repositoryMock = new Mock<IIndividualTrainingRepository>();
        _handler = new GymWebApp.Application.CQRS.IndividualTrainings.GetIndividualTrainingsFiltered.Handler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsFilteredTrainings()
    {
        var trainings = new List<IndividualTraining>
        {
            new() { Id = 1, TrainerId = 1, StartTime = DateTime.UtcNow.AddDays(1), EndTime = DateTime.UtcNow.AddDays(1).AddHours(1) },
            new() { Id = 2, TrainerId = 1, StartTime = DateTime.UtcNow.AddDays(2), EndTime = DateTime.UtcNow.AddDays(2).AddHours(1) }
        };

        _repositoryMock.Setup(x => x.GetFilteredIndividualTrainingsAsync(It.IsAny<IndividualTrainingFiltersDto>())).ReturnsAsync(trainings);

        var result = await _handler.Handle(new GetIndividualTrainingsFilteredQuery(null, null, "1", null), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoTrainings()
    {
        _repositoryMock.Setup(x => x.GetFilteredIndividualTrainingsAsync(It.IsAny<IndividualTrainingFiltersDto>())).ReturnsAsync(new List<IndividualTraining>());

        var result = await _handler.Handle(new GetIndividualTrainingsFilteredQuery(null, null, null, null), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
