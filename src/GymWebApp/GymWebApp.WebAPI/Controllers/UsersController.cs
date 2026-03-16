using GymWebApp.Application.CQRS.Users;
using GymWebApp.Application.WebModels.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/users")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserWebModel>>> GetUsers()
    {
        var users = await _mediator.Send(new GetUsersQuery());
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserWebModel>> CreateUser([FromBody] CreateUserCommand command)
    {
        var user = await _mediator.Send(command);
        return Ok(user);
    }

    [HttpGet("roles")]
    public async Task<ActionResult<List<string>>> GetRoles()
    {
        var roles = await _mediator.Send(new GetRolesQuery());
        return Ok(roles);
    }
}
