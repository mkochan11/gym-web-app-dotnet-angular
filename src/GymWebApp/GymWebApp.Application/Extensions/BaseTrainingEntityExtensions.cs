using GymWebApp.Domain.Entities.Abstract;
using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Extensions;

public static class BaseTrainingEntityExtensions
{
    public static EventStatus GetTrainingStatus(this BaseTrainingEntity training)
    {
        if (training.IsCancelled) return EventStatus.Cancelled;

        if (training.Date > DateTime.UtcNow)
        {
            return EventStatus.Scheduled;
        }
        else if (training.Date <= DateTime.UtcNow && training.Date.Add(training.Duration) >= DateTime.UtcNow)
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
        training.SetModificationAuditInfo(updatedById);
    }

    public static void SetCancelledFalse(this BaseTrainingEntity training, string updatedById)
    {
        training.IsCancelled = false;
        training.CancellationReason = null;
        training.SetModificationAuditInfo(updatedById);
    }
}