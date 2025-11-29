using GymWebApp.Data.Entities.Abstract;
using GymWebApp.Data.Enums;

namespace GymWebApp.ApplicationCore.Extensions;

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
}