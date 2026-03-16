using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/employees")]
[ApiController]
[Authorize(Roles = "Admin,Manager")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeesController(
        IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    [HttpGet]
    public async Task<ActionResult> GetEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        var employeesWebModels = employees.Select(e => e.ToEmployeeWebModel()).ToList();
        return Ok(employeesWebModels);
    }

    [HttpGet("{id}/employments")]
    public async Task<ActionResult>GetEmployeeEmploymentsAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdWithEmploymentsAsync(id);
        if (employee == null)
            return NotFound();

        return Ok(employee.ToEmployeeWithEmploymentsWebModel());
    }
}