using GymWebApp.Domain.Entities.Abstract;
using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Extensions;

public static class BaseTrainingEntityExtensions
{
    public static IEnumerable<EventStatus> GetTrainingStatuses(this BaseTrainingEntity training)
    {
        if (training.IsCancelled)
        {
            yield return EventStatus.Cancelled;
        }

        if (training.EndTime < DateTime.UtcNow)
        {
            yield return EventStatus.Completed;
        }
        else if (training.StartTime <= DateTime.UtcNow && training.EndTime >= DateTime.UtcNow)
        {
            yield return EventStatus.Ongoing;
        }

        if (training.StartTime > DateTime.UtcNow)
        {
            yield return EventStatus.Scheduled;
        }
    }

    public static void SetCancelledTrue(this BaseTrainingEntity training, string cancellationReason, string updatedById)
    {
        training.IsCancelled = true;
        training.CancellationReason = cancellationReason;
        training.CancelledAt = DateTime.UtcNow;
        training.SetModificationAuditInfo(updatedById);
    }

    public static void SetRemovedTrue(this BaseTrainingEntity training, string updatedById)
    {
        training.Removed = true;
        training.SetModificationAuditInfo(updatedById);
    }

    public static void SetCancelledFalse(this BaseTrainingEntity training, string updatedById)
    {
        training.IsCancelled = false;
        training.CancellationReason = null;
        training.CancelledAt = null;
        training.SetModificationAuditInfo(updatedById);
    }
}