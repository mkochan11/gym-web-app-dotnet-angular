using GymWebApp.Application.WebModels.Shift;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Extensions;

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

    public static void SetCancelledTrue(this Shift shift, string cancellationReason, string updatedById)
    {
        shift.IsCancelled = true;
        shift.CancellationReason = cancellationReason;
        shift.SetModificationAuditInfo(updatedById);
    }

    public static void SetCancelledFalse(this Shift shift, string updatedById)
    {
        shift.IsCancelled = false;
        shift.CancellationReason = null;
        shift.SetModificationAuditInfo(updatedById);
    }
}