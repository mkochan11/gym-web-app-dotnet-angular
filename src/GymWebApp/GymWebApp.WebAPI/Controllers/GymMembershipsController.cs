using GymWebApp.Application.CQRS.GymMemberships;
using GymWebApp.Application.WebModels.GymMembership;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/gym-memberships")]
[ApiController]
[Authorize]
public class GymMembershipsController : BaseController
{
    private readonly IMediator _mediator;

    public GymMembershipsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GymMembershipWebModel>> GetMembershipById(int id)
    {
        var membership = await _mediator.Send(new GetMembershipById.Query { Id = id });
        return Ok(membership);
    }

    [HttpGet("client/{clientId}")]
    public async Task<ActionResult<List<GymMembershipWebModel>>> GetClientMemberships(int clientId)
    {
        var membership = await _mediator.Send(new GetActiveMembership.Query { ClientId = clientId });
        if (membership == null)
        {
            return Ok(new List<GymMembershipWebModel>());
        }
        return Ok(new List<GymMembershipWebModel> { membership });
    }

    [HttpGet("client/{clientId}/active")]
    public async Task<ActionResult<GymMembershipWebModel>> GetActiveMembership(int clientId)
    {
        var membership = await _mediator.Send(new GetActiveMembership.Query { ClientId = clientId });
        if (membership == null)
        {
            return NotFound();
        }
        return Ok(membership);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<GymMembershipWebModel>> CancelMembership(int id, [FromBody] CancelMembership.Command command)
    {
        command.MembershipId = id;
        command.UpdatedById = CurrentUserId;
        var membership = await _mediator.Send(command);
        return Ok(membership);
    }

    [HttpPost("{id}/cancel/revert")]
    public async Task<ActionResult<GymMembershipWebModel>> RevertCancellation(int id)
    {
        var command = new RevertCancellation.Command
        {
            MembershipId = id,
            UpdatedById = CurrentUserId
        };
        var membership = await _mediator.Send(command);
        return Ok(membership);
    }

    [HttpPost("purchase")]
    public async Task<ActionResult<GymMembershipWebModel>> PurchaseMembership([FromBody] PurchaseMembership.Command command)
    {
        command.UpdatedById = CurrentUserId;
        var membership = await _mediator.Send(command);
        return Ok(membership);
    }
}
