using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.UpdateUser;

public class UpdateUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Users.UpdateUser.Handler _handler;

    public UpdateUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _handler = new GymWebApp.Application.CQRS.Users.UpdateUser.Handler(
            _userRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _employeeRepositoryMock.Object);
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

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync(userId))
            .ReturnsAsync((Client?)null);

        _employeeRepositoryMock.Setup(x => x.GetByAccountIdAsync(userId))
            .ReturnsAsync((Employee?)null);

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
            .WithMessage("Entity \"User\" (non-existing-id) was not found.");
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

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync(userId))
            .ReturnsAsync((Client?)null);

        _employeeRepositoryMock.Setup(x => x.GetByAccountIdAsync(userId))
            .ReturnsAsync((Employee?)null);

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
    public async Task Handle_ClientToTrainerRoleChange_RemovesClientAndCreatesEmployee()
    {
        var userId = "user-id-456";
        var command = new UpdateUserCommand
        {
            Id = userId,
            Email = "changed@example.com",
            FirstName = "Changed",
            LastName = "User",
            Role = "Trainer"
        };

        var existingUser = new UserWebModel
        {
            Id = userId,
            Email = "old@example.com",
            FirstName = "Old",
            LastName = "User",
            Role = "Client"
        };

        var updatedUser = new UserWebModel
        {
            Id = userId,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Role = "Trainer"
        };

        var existingClient = new Client { Id = 1, AccountId = userId, Name = "Old" };

        _userRepositoryMock.SetupSequence(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser)
            .ReturnsAsync(updatedUser);

        _userRepositoryMock.Setup(x => x.UpdateAsync(
            userId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        _clientRepositoryMock.Setup(x => x.GetByAccountIdAsync(userId))
            .ReturnsAsync(existingClient);

        _employeeRepositoryMock.Setup(x => x.GetByAccountIdAsync(userId))
            .ReturnsAsync((Employee?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Role.Should().Be("Trainer");
        _clientRepositoryMock.Verify(x => x.Remove(It.IsAny<Client>()), Times.Once);
        _employeeRepositoryMock.Verify(x => x.AddAsync(It.Is<Employee>(e => 
            e.AccountId == userId && 
            e.Role == EmployeeRole.Trainer)), Times.Once);
    }
}
