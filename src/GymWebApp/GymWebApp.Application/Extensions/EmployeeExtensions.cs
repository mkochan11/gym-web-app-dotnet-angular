using GymWebApp.Application.WebModels.Employee;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Extensions;

public static class EmployeeExtensions
{
    public static EmployeeWebModel ToEmployeeWebModel(this Employee employee)
    {
        if (employee == null) return null!;

        return new EmployeeWebModel
        {
            Id = employee.Id,
            FirstName = employee.Name,
            LastName = employee.Surname,
            Role = employee.Role.ToString()
        };
    }

    public static bool IsEmployeeActive(this Employee employee)
    {
        if (employee == null) return false;
        return employee.Employments.Any(e => e.EndDate == null || e.EndDate > DateTime.UtcNow);
    }
}