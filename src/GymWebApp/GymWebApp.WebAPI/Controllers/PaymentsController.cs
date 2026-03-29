using GymWebApp.Application.CQRS.Payments;
using GymWebApp.Application.WebModels.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/payments")]
[ApiController]
[Authorize]
public class PaymentsController : BaseController
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("membership/{membershipId}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByMembership(int membershipId)
    {
        var query = new GetPaymentsByMembership.Query
        {
            MembershipId = membershipId,
            UserId = CurrentUserId
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("process")]
    public async Task<ActionResult<PaymentResultWebModel>> ProcessPayment([FromBody] ProcessPayment.Command command)
    {
        command.CreatedById = CurrentUserId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("process-multiple")]
    public async Task<ActionResult<IEnumerable<PaymentResultWebModel>>> ProcessMultiplePayments([FromBody] ProcessMultiplePayments.Command command)
    {
        command.CreatedById = CurrentUserId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("client/{clientId}/schedule")]
    [Authorize(Roles = "Receptionist,Manager,Admin")]
    public async Task<ActionResult<ClientPaymentScheduleDto>> GetClientPaymentSchedule(int clientId)
    {
        var query = new GetClientPaymentSchedule.Query
        {
            ClientId = clientId
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("accept")]
    [Authorize(Roles = "Receptionist,Manager,Admin")]
    public async Task<ActionResult<PaymentResultWebModel>> AcceptPayment([FromBody] AcceptPayment.Command command)
    {
        command.ProcessedById = CurrentUserId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}