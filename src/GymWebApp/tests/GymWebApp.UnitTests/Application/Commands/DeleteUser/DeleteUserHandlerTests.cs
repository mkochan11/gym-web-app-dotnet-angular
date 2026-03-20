using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands;

public class DeleteUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly DeleteUser.Handler _handler;

    public DeleteUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _handler = new DeleteUser.Handler(
            _userRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _employeeRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsTrue()
    {
        var userId = "user-id-123";
        var command = new DeleteUserCommand { UserId = userId };

        var existingUser = new UserWebModel
        {
            Id = userId,
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.GetCurrentUserIdAsync())
            .ReturnsAsync("other-user-id");

        _userRepositoryMock.Setup(x => x.DeleteAsync(userId))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.DeleteAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        var userId = "non-existing-id";
        var command = new DeleteUserCommand { UserId = userId };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((UserWebModel?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"User\" ({userId}) was not found.");
    }

    [Fact]
    public async Task Handle_DeleteOwnAccount_ThrowsValidationException()
    {
        var userId = "current-user-id";
        var command = new DeleteUserCommand { UserId = userId };

        var existingUser = new UserWebModel
        {
            Id = userId,
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            Role = "Admin"
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.GetCurrentUserIdAsync())
            .ReturnsAsync(userId);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Cannot delete your own account");
    }

    [Fact]
    public async Task Handle_RepositoryFails_ReturnsFalse()
    {
        var userId = "user-id-123";
        var command = new DeleteUserCommand { UserId = userId };

        var existingUser = new UserWebModel
        {
            Id = userId,
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.GetCurrentUserIdAsync())
            .ReturnsAsync("other-user-id");

        _userRepositoryMock.Setup(x => x.DeleteAsync(userId))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ClientRole_CallsClientSoftDelete()
    {
        var userId = "client-user-id";
        var command = new DeleteUserCommand { UserId = userId };

        var existingUser = new UserWebModel
        {
            Id = userId,
            Email = "client@example.com",
            FirstName = "Client",
            LastName = "User",
            Role = "Client"
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.GetCurrentUserIdAsync())
            .ReturnsAsync("other-user-id");

        _userRepositoryMock.Setup(x => x.DeleteAsync(userId))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _clientRepositoryMock.Verify(x => x.SoftDeleteByAccountIdAsync(userId), Times.Once);
        _clientRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _employeeRepositoryMock.Verify(x => x.SoftDeleteByAccountIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TrainerRole_CallsEmployeeSoftDelete()
    {
        var userId = "trainer-user-id";
        var command = new DeleteUserCommand { UserId = userId };

        var existingUser = new UserWebModel
        {
            Id = userId,
            Email = "trainer@example.com",
            FirstName = "Trainer",
            LastName = "User",
            Role = "Trainer"
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.GetCurrentUserIdAsync())
            .ReturnsAsync("other-user-id");

        _userRepositoryMock.Setup(x => x.DeleteAsync(userId))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _employeeRepositoryMock.Verify(x => x.SoftDeleteByAccountIdAsync(userId), Times.Once);
        _employeeRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _clientRepositoryMock.Verify(x => x.SoftDeleteByAccountIdAsync(It.IsAny<string>()), Times.Never);
    }
}
