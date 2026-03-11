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
    
    public static EmployeeWithEmploymentsWebModel ToEmployeeWithEmploymentsWebModel(this Employee employee)
    {
        if (employee == null) return null!;
        
        List<EmploymentWebModel> employmentWebModels = [];

        foreach (var employment in employee.Employments)
        {
            employmentWebModels.Add(employment.ToEmploymentWebModel());
        }

        return new EmployeeWithEmploymentsWebModel()
        {
            Id = employee.Id,
            FirstName = employee.Name,
            LastName = employee.Surname,
            Role = employee.Role.ToString(),
            Employments = employmentWebModels
        };
    }

    public static bool IsEmployeeActive(this Employee employee)
    {
        if (employee == null) return false;
        return employee.Employments.Any(e => e.EndDate == null || e.EndDate > DateTime.UtcNow);
    }
}