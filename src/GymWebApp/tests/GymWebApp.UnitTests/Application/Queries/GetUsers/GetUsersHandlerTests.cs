using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.GetUsers;

public class GetUsersHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Users.GetUsers.Handler _handler;

    public GetUsersHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GymWebApp.Application.CQRS.Users.GetUsers.Handler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllUsers()
    {
        var users = new List<UserWebModel>
        {
            new() { Id = "1", Email = "user1@example.com", FirstName = "John", LastName = "Doe" },
            new() { Id = "2", Email = "user2@example.com", FirstName = "Jane", LastName = "Smith" }
        };

        _userRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(users);

        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoUsers()
    {
        _userRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<UserWebModel>());

        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
