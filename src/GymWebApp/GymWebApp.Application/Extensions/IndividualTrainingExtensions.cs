using GymWebApp.Application.WebModels.Training;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Extensions;

public static class IndividualTrainingExtensions
{
    public static IndividualTrainingWebModel ToIndividualTrainingWebModel(this IndividualTraining individualTraining)
    {
        if (individualTraining == null) return null!;

        return new IndividualTrainingWebModel
        {
            Id = individualTraining.Id,
            Description = individualTraining.Description,
            Date = individualTraining.Date.ToLocalTime(),
            Duration = individualTraining.Duration,
            Status = individualTraining.GetTrainingStatus().ToString(),
            Trainer = individualTraining.Trainer.ToTrainerWebModel(),
            Client = individualTraining.Client?.ToClientWebModel(),
        };
    }
}