using GymWebApp.ApplicationCore.Models.Training;
using GymWebApp.Data.Entities;

namespace GymWebApp.ApplicationCore.Extensions;

public static class GroupTrainingExtensions
{
    public static GroupTrainingWebModel ToGroupTrainingWebModel(this GroupTraining groupTraining)
    {
        if (groupTraining == null) return null!;

        return new GroupTrainingWebModel
        {
            Id = groupTraining.Id,
            Description = groupTraining.Description,
            Date = groupTraining.Date,
            Duration = groupTraining.Duration,
            Status = groupTraining.GetTrainingStatus().ToString(),
            Trainer = groupTraining.Trainer.ToTrainerWebModel(),
            CurrentParticipantNumber = groupTraining.Participations?.Count ?? 0,
            MaxParticipantNumber = groupTraining.MaxParticipantNumber,
            DifficultyLevel = groupTraining.DifficultyLevel,
            TrainingType = groupTraining.TrainingType.ToTrainingTypeWebModel()
        };
    }
}