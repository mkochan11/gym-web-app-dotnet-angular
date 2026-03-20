using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.GetUserById;

public class GetUserByIdHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Users.GetUserById.Handler _handler;

    public GetUserByIdHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GymWebApp.Application.CQRS.Users.GetUserById.Handler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ReturnsUser()
    {
        var userId = "user-id-123";
        var expectedUser = new UserWebModel
        {
            Id = userId,
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "123456789",
            Role = "Client"
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        var query = new GetUserByIdQuery { Id = userId };
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);
        result!.Id.Should().Be(userId);
        result.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task Handle_NonExistingUser_ReturnsNull()
    {
        var userId = "non-existing-id";
        
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((UserWebModel?)null);

        var query = new GetUserByIdQuery { Id = userId };
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}
