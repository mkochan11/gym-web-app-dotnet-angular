using System.Security.Claims;
using GymWebApp.Application.CQRS.GymMemberships;
using GymWebApp.Application.WebModels.GymMembership;
using GymWebApp.Application.WebModels.MembershipPlan;
using GymWebApp.Domain.Enums;
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
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        command.RequireCancellationReason = userRole is "Manager" or "Receptionist";
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

    [HttpGet("{id}/available-plans")]
    public async Task<ActionResult<List<MembershipPlanWebModel>>> GetAvailablePlans(int id)
    {
        var plans = await _mediator.Send(new GetAvailablePlans.Query { MembershipId = id });
        return Ok(plans);
    }

    [HttpGet("{id}/calculate-credit")]
    public async Task<ActionResult<CreditCalculationResult>> CalculateCredit(int id, [FromQuery] int newPlanId)
    {
        var result = await _mediator.Send(new CalculatePlanChangeCredit.Query { MembershipId = id, NewPlanId = newPlanId });
        return Ok(result);
    }

    [HttpPost("{id}/change-plan")]
    public async Task<ActionResult<GymMembershipWebModel>> ChangePlan(int id, [FromBody] ChangePlanRequest request)
    {
        var command = new ChangeMembershipPlan.Command
        {
            MembershipId = id,
            NewPlanId = request.NewPlanId,
            UpdatedById = CurrentUserId
        };
        var membership = await _mediator.Send(command);
        return Ok(membership);
    }

    [HttpPost("activate")]
    [Authorize(Roles = "Manager,Receptionist")]
    public async Task<ActionResult<GymMembershipWebModel>> ActivateMembership([FromBody] ActivateMembershipRequest request)
    {
        var command = new ActivateMembership.Command
        {
            ClientId = request.ClientId,
            MembershipPlanId = request.MembershipPlanId,
            PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod),
            TransactionId = request.TransactionId,
            Amount = request.Amount,
            UpdatedById = CurrentUserId
        };
        var membership = await _mediator.Send(command);
        return Ok(membership);
    }
}

public class ActivateMembershipRequest
{
    public int ClientId { get; set; }
    public int MembershipPlanId { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string? TransactionId { get; set; }
    public decimal Amount { get; set; }
}
