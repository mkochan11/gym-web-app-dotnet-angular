using GymWebApp.Data.Entities;
using GymWebApp.Data.Entities.Abstract;
using GymWebApp.Data.Enums;

namespace GymWebApp.ApplicationCore.Extensions;

public static class BaseTrainingEntityExtensions
{
    public static EventStatus GetTrainingStatus(this BaseTrainingEntity training)
    {
        if (training.Date > DateTime.UtcNow.AddHours(1))
        {
            return EventStatus.Scheduled;
        }
        else if (training.Date <= DateTime.UtcNow.AddHours(1) && training.Date.Add(training.Duration) >= DateTime.UtcNow.AddHours(1))
        {
            return EventStatus.Ongoing;
        }
        else
        {
            return EventStatus.Completed;
        }
    }
}