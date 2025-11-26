using GymWebApp.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GymWebApp.ApplicationCore.Extensions;
using GymWebApp.ApplicationCore.CQRS.Shift;
using GymWebApp.Data.DTOs;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/shifts")]
public class ShiftsController : ControllerBase
{
    private readonly IShiftRepository _shiftRepository;

    public ShiftsController(
        IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    [HttpGet]
    public async Task<ActionResult> GetShiftsAsync()
    {
        var shifts = await _shiftRepository.GetAllShiftsAsync();
        var shiftWebModels = shifts.Select(s => s.ToShiftWebModel()).ToList();

        return Ok(shiftWebModels);
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
}
