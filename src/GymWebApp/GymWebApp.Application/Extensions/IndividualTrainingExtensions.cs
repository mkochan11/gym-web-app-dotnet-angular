using GymWebApp.Application.WebModels.Training;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Extensions;

public static class IndividualTrainingExtensions
{
    public static CalendarIndividualTrainingWebModel ToCalendarIndividualTrainingWebModel(this IndividualTraining individualTraining)
    {
        if (individualTraining == null) return null!;

        return new CalendarIndividualTrainingWebModel
        {
            Id = individualTraining.Id,
            Description = individualTraining.Description,   
            Date = individualTraining.StartTime.ToLocalTime(),
            Duration = individualTraining.EndTime.ToLocalTime() - individualTraining.StartTime.ToLocalTime(),
            Statuses = individualTraining.GetTrainingStatuses().Select(s => s.ToString()).ToArray(),
            Trainer = individualTraining.Trainer.ToTrainerWebModel(),
            Client = individualTraining.Client?.ToClientWebModel(),
        };
    }

    public static IndividualTrainingWebModel ToIndividualTrainingWebModel(this IndividualTraining individualTraining)
    {
        if (individualTraining == null) return null!;

        return new IndividualTrainingWebModel
        {
            Id = individualTraining.Id,
            Description = individualTraining.Description,
            StartTime = individualTraining.StartTime.ToLocalTime(),
            Duration = individualTraining.EndTime.ToLocalTime() - individualTraining.StartTime.ToLocalTime(),
            Statuses = individualTraining.GetTrainingStatuses().Select(s => s.ToString()).ToArray(),
            Trainer = individualTraining.Trainer.ToTrainerWebModel(),
            Client = individualTraining.Client?.ToClientWebModel(),
            CancelledAt = individualTraining.CancelledAt?.ToLocalTime(),
            CreatedAt = individualTraining.CreatedAt.ToLocalTime(),
            UpdatedAt = individualTraining.CreatedAt.ToLocalTime(),
            EndTime = individualTraining.EndTime.ToLocalTime(),
            Removed = individualTraining.Removed
        };
    }
}