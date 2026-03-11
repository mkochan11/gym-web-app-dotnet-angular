using GymWebApp.Domain.Entities.Abstract;
using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Extensions;

public static class BaseTrainingEntityExtensions
{
    public static EventStatus GetTrainingStatus(this BaseTrainingEntity training)
    {
        if (training.IsCancelled) return EventStatus.Cancelled;

        if (training.StartTime > DateTime.UtcNow)
        {
            return EventStatus.Scheduled;
        }
        else if (training.StartTime <= DateTime.UtcNow && training.EndTime >= DateTime.UtcNow)
        {
            return EventStatus.Ongoing;
        }
        else
        {
            return EventStatus.Completed;
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