using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.CreateUser;

public class CreateUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Users.CreateUser.Handler _handler;

    public CreateUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _handler = new GymWebApp.Application.CQRS.Users.CreateUser.Handler(
            _userRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _employeeRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreatedUser()
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Password = "Password123",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var createdUser = new UserWebModel
        {
            Id = "user-id",
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Role = "Client"
        };

        var domainUser = new ApplicationUser { Id = "user-id", Email = command.Email, UserName = command.Email };

        ApplicationUser? nullUser = null;
        _userRepositoryMock.SetupSequence(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(nullUser)
            .ReturnsAsync(domainUser);

        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<CreateUserWebModel>()))
            .ReturnsAsync(default((ApplicationUser, string)?));

        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(createdUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be(command.Email);
        result.FirstName.Should().Be(command.FirstName);
        result.LastName.Should().Be(command.LastName);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsValidationException()
    {
        var command = new CreateUserCommand
        {
            Email = "existing@example.com",
            Password = "Password123",
            FirstName = "John",
            LastName = "Doe",
            Role = "Client"
        };

        var existingUser = new ApplicationUser { Id = "existing-id", Email = "existing@example.com", UserName = "existing@example.com" };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(existingUser);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Email already exists");
    }

    [Fact]
    public async Task Handle_ClientRole_CreatesClientEntity()
    {
        var command = new CreateUserCommand
        {
            Email = "client@example.com",
            Password = "Password123",
            FirstName = "Client",
            LastName = "User",
            Role = "Client"
        };

        var createdUser = new UserWebModel
        {
            Id = "client-user-id",
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Role = "Client"
        };

        var domainUser = new ApplicationUser { Id = "client-user-id", Email = command.Email, UserName = command.Email };

        ApplicationUser? nullUser = null;
        _userRepositoryMock.SetupSequence(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(nullUser)
            .ReturnsAsync(domainUser);

        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<CreateUserWebModel>()))
            .ReturnsAsync(default((ApplicationUser, string)?));

        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(createdUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        _clientRepositoryMock.Verify(x => x.AddAsync(It.Is<Client>(c => 
            c.AccountId == "client-user-id" && 
            c.Name == "Client" && 
            c.Surname == "User")), Times.Once);
    }

    [Fact]
    public async Task Handle_TrainerRole_CreatesEmployeeEntity()
    {
        var command = new CreateUserCommand
        {
            Email = "trainer@example.com",
            Password = "Password123",
            FirstName = "Trainer",
            LastName = "User",
            Role = "Trainer"
        };

        var createdUser = new UserWebModel
        {
            Id = "trainer-user-id",
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Role = "Trainer"
        };

        var domainUser = new ApplicationUser { Id = "trainer-user-id", Email = command.Email, UserName = command.Email };

        ApplicationUser? nullUser = null;
        _userRepositoryMock.SetupSequence(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync(nullUser)
            .ReturnsAsync(domainUser);

        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<CreateUserWebModel>()))
            .ReturnsAsync(default((ApplicationUser, string)?));

        _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(createdUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        _employeeRepositoryMock.Verify(x => x.AddAsync(It.Is<Employee>(e => 
            e.AccountId == "trainer-user-id" && 
            e.Name == "Trainer" && 
            e.Surname == "User" &&
            e.Role == EmployeeRole.Trainer)), Times.Once);
    }
}
