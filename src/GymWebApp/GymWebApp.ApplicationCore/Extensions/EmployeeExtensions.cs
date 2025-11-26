using GymWebApp.ApplicationCore.Models.Employee;
using GymWebApp.Data.Entities;

namespace GymWebApp.ApplicationCore.Extensions;

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
}