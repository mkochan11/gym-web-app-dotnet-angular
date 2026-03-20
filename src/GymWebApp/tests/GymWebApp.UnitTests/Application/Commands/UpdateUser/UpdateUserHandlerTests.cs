using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.UpdateUser;

public class UpdateUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Users.UpdateUser.Handler _handler;

    public UpdateUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GymWebApp.Application.CQRS.Users.UpdateUser.Handler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedUser()
    {
        var userId = "user-id-123";
        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "123456789",
            Role = "Trainer"
        };

        var existingUser = new UserWebModel
        {
            Id = userId,
            Email = "old@example.com",
            FirstName = "OldName",
            LastName = "OldLastName",
            Role = "Client"
        };

        var updatedUser = new UserWebModel
        {
            Id = userId,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PhoneNumber = command.PhoneNumber,
            Role = command.Role
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.UpdateAsync(
            userId, command.Email, command.FirstName, command.LastName, command.PhoneNumber, command.Role))
            .ReturnsAsync((string?)null);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(updatedUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Email.Should().Be(command.Email);
        result.FirstName.Should().Be(command.FirstName);
        result.LastName.Should().Be(command.LastName);
        result.Role.Should().Be(command.Role);
    }

    [Fact]
    public async Task Handle_NonExistingUser_ThrowsNotFoundException()
    {
        var command = new UpdateUserCommand
        {
            Id = "non-existing-id",
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync((UserWebModel?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User with id non-existing-id was not found");
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsValidationException()
    {
        var userId = "user-id-123";
        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = "existing@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

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

        _userRepositoryMock.Setup(x => x.EmailExistsAsync("existing@example.com"))
            .ReturnsAsync(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Email already exists");
    }

    [Fact]
    public async Task Handle_SameEmail_NoEmailDuplicateCheck()
    {
        var userId = "user-id-123";
        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var existingUser = new UserWebModel
        {
            Id = userId,
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var updatedUser = new UserWebModel
        {
            Id = userId,
            Email = command.Email,
            FirstName = "John Updated",
            LastName = command.LastName,
            Role = command.Role
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.UpdateAsync(
            userId, command.Email, command.FirstName, command.LastName, null, command.Role))
            .ReturnsAsync((string?)null);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(updatedUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        _userRepositoryMock.Verify(x => x.EmailExistsAsync(It.IsAny<string>()), Times.Never);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_UpdateFails_ThrowsValidationException()
    {
        var userId = "user-id-123";
        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

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

        _userRepositoryMock.Setup(x => x.UpdateAsync(
            userId, command.Email, command.FirstName, command.LastName, null, command.Role))
            .ReturnsAsync("Update failed due to database error");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("User update failed");
    }

    [Fact]
    public async Task Handle_RoleChange_UpdatesRole()
    {
        var userId = "user-id-123";
        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Admin"
        };

        var existingUser = new UserWebModel
        {
            Id = userId,
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var updatedUser = new UserWebModel
        {
            Id = userId,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Role = "Admin"
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(x => x.UpdateAsync(
            userId, command.Email, command.FirstName, command.LastName, null, command.Role))
            .ReturnsAsync((string?)null);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(updatedUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Role.Should().Be("Admin");
    }
}
