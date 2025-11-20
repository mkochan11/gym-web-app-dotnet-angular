using GymWebApp.ApplicationCore.Models.Training;
using GymWebApp.Data.Entities;

namespace GymWebApp.ApplicationCore.Extensions;

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