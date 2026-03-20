using GymWebApp.Application.CQRS.MembershipPlans;
using GymWebApp.Application.WebModels.MembershipPlan;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/membership-plans")]
[ApiController]
[Authorize]
public class MembershipPlansController : BaseController
{
    private readonly IMediator _mediator;

    public MembershipPlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<MembershipPlanWebModel>>> GetMembershipPlans()
    {
        var plans = await _mediator.Send(new GetMembershipPlansQuery());
        return Ok(plans);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MembershipPlanWebModel>> GetMembershipPlanById(int id)
    {
        var plan = await _mediator.Send(new GetMembershipPlanByIdQuery { Id = id });
        if (plan == null)
        {
            return NotFound();
        }
        return Ok(plan);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<ActionResult<MembershipPlanWebModel>> CreateMembershipPlan([FromBody] CreateMembershipPlanCommand command)
    {
        command.CreatedById = CurrentUserId;
        var plan = await _mediator.Send(command);
        return Ok(plan);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<ActionResult<MembershipPlanWebModel>> UpdateMembershipPlan(int id, [FromBody] UpdateMembershipPlanCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Membership Plan ID mismatch");
        }
        command.UpdatedById = CurrentUserId;
        var plan = await _mediator.Send(command);
        return Ok(plan);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> DeleteMembershipPlan(int id)
    {
        await _mediator.Send(new DeleteMembershipPlanCommand { Id = id });
        return NoContent();
    }
}
