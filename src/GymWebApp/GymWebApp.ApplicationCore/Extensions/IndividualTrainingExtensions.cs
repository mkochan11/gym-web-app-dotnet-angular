using GymWebApp.ApplicationCore.Models.Training;
using GymWebApp.Data.Entities;

namespace GymWebApp.ApplicationCore.Extensions;

public static class IndividualTrainingExtensions
{
    public static IndividualTrainingWebModel ToIndividualTrainingWebModel(this IndividualTraining individualTraining)
    {
        if (individualTraining == null) return null!;

        return new IndividualTrainingWebModel
        {
            Id = individualTraining.Id,
            Description = individualTraining.Description,
            Date = individualTraining.Date,
            Duration = individualTraining.Duration,
            IsCompleted = individualTraining.IsCompleted,
            IsCancelled = individualTraining.IsCancelled,
            Trainer = individualTraining.Trainer.ToTrainerWebModel(),
            Client = individualTraining.Client?.ToClientWebModel(),
        };
    }
}