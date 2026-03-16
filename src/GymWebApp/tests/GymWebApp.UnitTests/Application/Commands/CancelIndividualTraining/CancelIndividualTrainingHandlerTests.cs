using FluentAssertions;
using GymWebApp.Application.CQRS.IndividualTrainings;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.UnitTests.Application.Commands.CancelIndividualTraining;

public class CancelIndividualTrainingHandlerTests
{
    private readonly Mock<IIndividualTrainingRepository> _repositoryMock;
    private readonly GymWebApp.Application.CQRS.IndividualTrainings.CancelIndividualTraining.Handler _handler;

    public CancelIndividualTrainingHandlerTests()
    {
        _repositoryMock = new Mock<IIndividualTrainingRepository>();
        _handler = new GymWebApp.Application.CQRS.IndividualTrainings.CancelIndividualTraining.Handler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CancelsTraining()
    {
        var training = new IndividualTraining { Id = 1, IsCancelled = false };
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);
        _repositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(new CancelIndividualTrainingCommand(1, "Sick", "admin-id"), CancellationToken.None);

        training.IsCancelled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((IndividualTraining?)null);

        var act = () => _handler.Handle(new CancelIndividualTrainingCommand(999, "Sick", "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_AlreadyCancelled_ThrowsValidationException()
    {
        var training = new IndividualTraining { Id = 1, IsCancelled = true };
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);

        var act = () => _handler.Handle(new CancelIndividualTrainingCommand(1, "Sick", "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
