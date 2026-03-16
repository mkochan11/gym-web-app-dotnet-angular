using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;
using BusinessRuleViolationException = GymWebApp.Application.Common.Exceptions.BusinessRuleViolationException;

namespace GymWebApp.UnitTests.Application.Commands.RestoreIndividualTraining;

public class RestoreIndividualTrainingHandlerTests
{
    private readonly Mock<IIndividualTrainingRepository> _individualRepoMock;
    private readonly Mock<IGroupTrainingRepository> _groupRepoMock;
    private readonly Mock<IShiftRepository> _shiftRepoMock;
    private readonly GymWebApp.Application.CQRS.IndividualTrainings.RestoreIndividualTraining.Handler _handler;

    public RestoreIndividualTrainingHandlerTests()
    {
        _individualRepoMock = new Mock<IIndividualTrainingRepository>();
        _groupRepoMock = new Mock<IGroupTrainingRepository>();
        _shiftRepoMock = new Mock<IShiftRepository>();
        _handler = new GymWebApp.Application.CQRS.IndividualTrainings.RestoreIndividualTraining.Handler(
            _individualRepoMock.Object, _groupRepoMock.Object, _shiftRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_RestoresTraining()
    {
        var training = new IndividualTraining { Id = 1, TrainerId = 1, StartTime = DateTime.UtcNow.AddDays(2), IsCancelled = true };
        _individualRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);
        _individualRepoMock.Setup(x => x.ExistsOverlappingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _groupRepoMock.Setup(x => x.ExistsOverlappingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _shiftRepoMock.Setup(x => x.ExistsOverlappingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _individualRepoMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(new RestoreIndividualTrainingCommand(1, "admin-id"), CancellationToken.None);

        training.IsCancelled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _individualRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((IndividualTraining?)null);

        var act = () => _handler.Handle(new RestoreIndividualTrainingCommand(999, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NotCancelled_ThrowsBusinessRuleViolationException()
    {
        var training = new IndividualTraining { Id = 1, IsCancelled = false };
        _individualRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);

        var act = () => _handler.Handle(new RestoreIndividualTrainingCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>();
    }

    [Fact]
    public async Task Handle_PastTraining_ThrowsBusinessRuleViolationException()
    {
        var training = new IndividualTraining { Id = 1, StartTime = DateTime.UtcNow.AddDays(-1), IsCancelled = true };
        _individualRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);

        var act = () => _handler.Handle(new RestoreIndividualTrainingCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>();
    }
}
