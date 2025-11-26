using GymWebApp.ApplicationCore.Extensions;
using GymWebApp.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/employees")]
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
}