using FluentAssertions;
using GymWebApp.Application.CQRS.GroupTrainings;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Moq;
using Xunit;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.UnitTests.Application.Commands.DeleteGroupTraining;

public class DeleteGroupTrainingHandlerTests
{
    private readonly Mock<IGroupTrainingRepository> _repositoryMock;
    private readonly GymWebApp.Application.CQRS.GroupTrainings.DeleteGroupTraining.Handler _handler;

    public DeleteGroupTrainingHandlerTests()
    {
        _repositoryMock = new Mock<IGroupTrainingRepository>();
        _handler = new GymWebApp.Application.CQRS.GroupTrainings.DeleteGroupTraining.Handler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_CancelledTraining_DeletesTraining()
    {
        var training = new GroupTraining { Id = 1, IsCancelled = true };
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);
        _repositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(new DeleteGroupTrainingCommand(1, "admin-id"), CancellationToken.None);

        _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((GroupTraining?)null);

        var act = () => _handler.Handle(new DeleteGroupTrainingCommand(999, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NotCancelled_ThrowsValidationException()
    {
        var training = new GroupTraining { Id = 1, IsCancelled = false };
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(training);

        var act = () => _handler.Handle(new DeleteGroupTrainingCommand(1, "admin-id"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
