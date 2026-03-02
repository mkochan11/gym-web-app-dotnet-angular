using GymWebApp.Application.WebModels.Client;
using GymWebApp.Application.WebModels.Trainer;

namespace GymWebApp.Application.WebModels.Training;

public class IndividualTrainingWebModel
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public TimeSpan Duration { get; set; }

    public string Status { get; set; } = null!;

    public TrainerWebModel Trainer { get; set; } = null!;

    public ClientWebModel? Client { get; set; }
}