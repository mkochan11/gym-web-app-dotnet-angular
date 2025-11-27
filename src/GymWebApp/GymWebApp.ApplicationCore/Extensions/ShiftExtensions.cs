using GymWebApp.ApplicationCore.Models.Shift;
using GymWebApp.Data.Entities;
using GymWebApp.Data.Enums;

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
            Employee = shift.Employee.ToEmployeeWebModel(),
            Status = shift.GetShiftStatus().ToString()
        };
    }

    private static EventStatus GetShiftStatus(this Shift shift)
    {
        if (shift.StartTime > DateTime.Now)
        {
            return EventStatus.Scheduled;
        }
        else if (shift.StartTime <= DateTime.Now && shift.EndTime >= DateTime.Now)
        {
            return EventStatus.Ongoing;
        }
        else
        {
            return EventStatus.Completed;
        }
    }
}