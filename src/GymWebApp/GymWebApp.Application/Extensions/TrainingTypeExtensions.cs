using GymWebApp.Application.WebModels.Training;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Extensions;

public static class TrainingTypeExtensions
{
    public static TrainingTypeWebModel ToTrainingTypeWebModel(this TrainingType trainingType) =>
        new TrainingTypeWebModel
        {
            Id = trainingType.Id,
            Name = trainingType.Name,
            Description = trainingType.Description,
            DifficultyLevel = trainingType.DifficultyLevel
        };
}