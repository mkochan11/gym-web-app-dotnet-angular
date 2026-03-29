using GymWebApp.Application.CQRS.Users;
using GymWebApp.Application.WebModels.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/users")]
[ApiController]
[Authorize(Roles = "Admin,Manager")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string? GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserWebModel>>> GetUsers()
    {
        var query = new GetUsersQuery
        {
            CurrentUserRole = GetCurrentUserRole()
        };
        var users = await _mediator.Send(query);
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserWebModel>> CreateUser([FromBody] CreateUserCommand command)
    {
        command.CurrentUserRole = GetCurrentUserRole();
        var user = await _mediator.Send(command);
        return Ok(user);
    }

    [HttpGet("roles")]
    public async Task<ActionResult<List<string>>> GetRoles()
    {
        var query = new GetRolesQuery
        {
            CurrentUserRole = GetCurrentUserRole()
        };
        var roles = await _mediator.Send(query);
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserWebModel>> GetUserById(string id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserWebModel>> UpdateUser(string id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("User ID mismatch");
        }
        command.CurrentUserRole = GetCurrentUserRole();
        var user = await _mediator.Send(command);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var command = new DeleteUserCommand
        {
            UserId = id,
            CurrentUserRole = GetCurrentUserRole()
        };
        var result = await _mediator.Send(command);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
