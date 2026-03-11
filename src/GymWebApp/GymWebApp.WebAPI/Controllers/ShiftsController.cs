using GymWebApp.Application.CQRS.Shifts;
using GymWebApp.Application.DTOs;
using GymWebApp.Application.DTOs.Events;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/shifts")]
public class ShiftsController : BaseController
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IMediator _mediator;

    public ShiftsController(
        IShiftRepository shiftRepository,
        IMediator mediator)
    {
        _shiftRepository = shiftRepository;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetShiftsAsync()
    {
        var shifts = await _shiftRepository.GetAllShiftsAsync();
        var shiftWebModels = shifts.Select(s => s.ToShiftWebModel()).ToList();

        return Ok(shiftWebModels);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<int>> CreateShiftAsync([FromBody] CreateShiftCommand command)
    {
        command.CreatedById = CurrentUserId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("filtered")]
    public async Task<ActionResult> GetFilteredShiftsAsync([FromQuery] GetShiftsFilteredQuery query)
    {
        var shiftFiltersDto = new ShiftFiltersDto
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            EmployeeIds = query.EmployeeIds.ToIntList(),
        };

        var shifts = await _shiftRepository.GetFilteredShiftsAsync(shiftFiltersDto);
        var shiftWebModels = shifts.Select(s => s.ToShiftWebModel()).ToList();

        return Ok(shiftWebModels);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelShift(int id, [FromBody] CancelEventRequest request)
    {
        var command = new CancelShiftCommand(
            id,
            request.CancellationReason,
            CurrentUserId);

        await _mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> DeleteShiftAsync(int id)
    {
        var command = new DeleteShiftCommand(
            id,
            CurrentUserId
        );

        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("{id}/restore")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> RestoreShiftAsync(int id)
    {
        var command = new RestoreShiftCommand(
            id,
            CurrentUserId
        );

        await _mediator.Send(command);
        return Ok();
    }
}
