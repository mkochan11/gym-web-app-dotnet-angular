using FluentAssertions;
using GymWebApp.Application.CQRS.Users;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Queries.GetRoles;

public class GetRolesHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GymWebApp.Application.CQRS.Users.GetRoles.Handler _handler;

    public GetRolesHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GymWebApp.Application.CQRS.Users.GetRoles.Handler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllRoles()
    {
        var roles = new List<UserRole> { UserRole.Client, UserRole.Trainer, UserRole.Manager, UserRole.Admin };

        _userRepositoryMock.Setup(x => x.GetAllRolesAsync())
            .ReturnsAsync(roles);

        var result = await _handler.Handle(new GetRolesQuery(), CancellationToken.None);

        result.Should().HaveCount(4);
        result.Should().Contain("Client");
        result.Should().Contain("Trainer");
        result.Should().Contain("Manager");
        result.Should().Contain("Admin");
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoRoles()
    {
        _userRepositoryMock.Setup(x => x.GetAllRolesAsync())
            .ReturnsAsync(new List<UserRole>());

        var result = await _handler.Handle(new GetRolesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
