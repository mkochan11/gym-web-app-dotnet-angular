using GymWebApp.Application.WebModels.Trainer;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Extensions;

public static class TrainerExtensions
{
    public static TrainerWebModel ToTrainerWebModel(this Employee trainer)
    {
        if (trainer == null) return null!;

        return new TrainerWebModel
        {
            Id = trainer.Id,
            FirstName = trainer.Name,
            LastName = trainer.Surname,
        };
    }
}