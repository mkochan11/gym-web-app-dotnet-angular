using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;
using BusinessRuleViolationException = GymWebApp.Application.Common.Exceptions.BusinessRuleViolationException;

namespace GymWebApp.UnitTests.Application.Commands.RestoreGroupTraining;

public class RestoreGroupTrainingHandlerTests
{
    private readonly Mock<IGroupTrainingRepository> _groupRepoMock;
    private readonly Mock<IIndividualTrainingRepository> _individualRepoMock;
    private readonly Mock<IShiftRepository> _shiftRepoMock;
    private readonly GymWebApp.Application.CQRS.GroupTrainings.RestoreGroupTraining.Handler _handler;

    public RestoreGroupTrainingHandlerTests()
    {
        _groupRepoMock = new Mock<IGroupTrainingRepository>();
        _individualRepoMock = new Mock<IIndividualTrainingRepository>();
        _shiftRepoMock = new Mock<IShiftRepository>();
        _handler = new GymWebApp.Application.CQRS.GroupTrainings.RestoreGroupTraining.Handler(
            _groupRepoMock.Object, _individualRepoMock.Object, _shiftRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_RestoresTraining()
    {
        var training = new GroupTraining { Id = 1, TrainerId = 1, StartTime = DateTime.UtcNow.AddDays(2), IsCancelled = true };
        _groupRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);
        _groupRepoMock.Setup(x => x.ExistsOverlappingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _individualRepoMock.Setup(x => x.ExistsOverlappingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _shiftRepoMock.Setup(x => x.ExistsOverlappingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _groupRepoMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(new RestoreGroupTrainingCommand(1, "admin-id"), CancellationToken.None);

        training.IsCancelled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _groupRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((GroupTraining?)null);

        var act = () => _handler.Handle(new RestoreGroupTrainingCommand(999, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NotCancelled_ThrowsBusinessRuleViolationException()
    {
        var training = new GroupTraining { Id = 1, IsCancelled = false };
        _groupRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);

        var act = () => _handler.Handle(new RestoreGroupTrainingCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>();
    }

    [Fact]
    public async Task Handle_PastTraining_ThrowsBusinessRuleViolationException()
    {
        var training = new GroupTraining { Id = 1, StartTime = DateTime.UtcNow.AddDays(-1), IsCancelled = true };
        _groupRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);

        var act = () => _handler.Handle(new RestoreGroupTrainingCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>();
    }
}
