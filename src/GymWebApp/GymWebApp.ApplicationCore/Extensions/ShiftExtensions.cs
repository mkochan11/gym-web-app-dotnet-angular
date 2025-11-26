using GymWebApp.ApplicationCore.Models.Shift;
using GymWebApp.Data.Entities;

namespace GymWebApp.ApplicationCore.Extensions;

public static class ShiftExtensions
{
    public static ShiftWebModel ToShiftWebModel(this Shift shift)
    {
        if (shift == null) return null!;

        return new ShiftWebModel
        {
            Id = shift.Id,
            StartDate = shift.StartTime,
            EndDate = shift.EndTime,
            Employee = shift.Employee.ToEmployeeWebModel()
        };
    }
}