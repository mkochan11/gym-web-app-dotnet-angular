using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.UnitTests.Application.Commands.DeleteIndividualTraining;

public class DeleteIndividualTrainingHandlerTests
{
    private readonly Mock<IIndividualTrainingRepository> _repositoryMock;
    private readonly GymWebApp.Application.CQRS.IndividualTrainings.DeleteIndividualTraining.Handler _handler;

    public DeleteIndividualTrainingHandlerTests()
    {
        _repositoryMock = new Mock<IIndividualTrainingRepository>();
        _handler = new GymWebApp.Application.CQRS.IndividualTrainings.DeleteIndividualTraining.Handler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_CancelledTraining_DeletesTraining()
    {
        var training = new IndividualTraining { Id = 1, IsCancelled = true };
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);
        _repositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(new DeleteIndividualTrainingCommand(1, "admin-id"), CancellationToken.None);

        _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((IndividualTraining?)null);

        var act = () => _handler.Handle(new DeleteIndividualTrainingCommand(999, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NotCancelled_ThrowsValidationException()
    {
        var training = new IndividualTraining { Id = 1, IsCancelled = false };
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);

        var act = () => _handler.Handle(new DeleteIndividualTrainingCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
