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
            Statuses = shift.GetShiftStatuses().Select(s => s.ToString()).ToArray()
        };
    }

    public static IEnumerable<EventStatus> GetShiftStatuses(this Shift shift)
    {
        if (shift.IsCancelled)
        {
            yield return EventStatus.Cancelled;
        }

        if (shift.EndTime < DateTime.Now)
        {
            yield return EventStatus.Completed;
        }
        else if (shift.StartTime <= DateTime.Now && shift.EndTime >= DateTime.Now)
        {
            yield return EventStatus.Ongoing;
        }

        if (shift.StartTime > DateTime.Now)
        {
            yield return EventStatus.Scheduled;
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

    public static void SetRemovedTrue(this Shift shift, string updatedById)
    {
        shift.Removed = true;
        shift.SetModificationAuditInfo(updatedById);
    }
}