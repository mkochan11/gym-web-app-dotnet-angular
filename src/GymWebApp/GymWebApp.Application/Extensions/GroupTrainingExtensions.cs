using GymWebApp.Application.WebModels.Training;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Extensions;

public static class GroupTrainingExtensions
{
    public static CalendarGroupTrainingWebModel ToCalendarGroupTrainingWebModel(this GroupTraining groupTraining)
    {
        if (groupTraining == null) return null!;

        return new CalendarGroupTrainingWebModel
        {
            Id = groupTraining.Id,
            Description = groupTraining.Description,
            Date = groupTraining.StartTime.ToLocalTime(),
            Duration = groupTraining.EndTime.ToLocalTime() - groupTraining.StartTime.ToLocalTime(),
            Statuses = groupTraining.GetTrainingStatuses().Select(s => s.ToString()).ToArray(),
            Trainer = groupTraining.Trainer.ToTrainerWebModel(),
            CurrentParticipantNumber = groupTraining.Participations?.Count ?? 0,
            MaxParticipantNumber = groupTraining.MaxParticipantNumber,
            DifficultyLevel = groupTraining.DifficultyLevel,
            TrainingType = groupTraining.TrainingType.ToTrainingTypeWebModel(),
            
        };
    }

    public static GroupTrainingWebModel ToGroupTrainingWebModel(this GroupTraining groupTraining)
    {
        if (groupTraining == null) return null!;

        return new GroupTrainingWebModel
        {
            Id = groupTraining.Id,
            Description = groupTraining.Description,
            StartTime = groupTraining.StartTime.ToLocalTime(),
            Duration = groupTraining.EndTime.ToLocalTime() - groupTraining.StartTime.ToLocalTime(),
            Statuses = groupTraining.GetTrainingStatuses().Select(s => s.ToString()).ToArray(),
            Trainer = groupTraining.Trainer.ToTrainerWebModel(),
            CurrentParticipantNumber = groupTraining.Participations?.Count ?? 0,
            MaxParticipantNumber = groupTraining.MaxParticipantNumber,
            DifficultyLevel = groupTraining.DifficultyLevel,
            TrainingType = groupTraining.TrainingType.ToTrainingTypeWebModel(),
            CancelledAt = groupTraining.CancelledAt?.ToLocalTime(),
            CreatedAt = groupTraining.CreatedAt.ToLocalTime(),
            UpdatedAt = groupTraining.CreatedAt.ToLocalTime(),
            EndTime = groupTraining.EndTime.ToLocalTime(),
            Removed = groupTraining.Removed
        };
    }
}