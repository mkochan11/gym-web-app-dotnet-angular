using GymWebApp.ApplicationCore.Models.Client;
using GymWebApp.ApplicationCore.Models.Trainer;

namespace GymWebApp.ApplicationCore.Models.Training;

public class IndividualTrainingWebModel
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public TimeSpan Duration { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsCancelled { get; set; }

    public TrainerWebModel Trainer { get; set; } = null!;

    public ClientWebModel? Client { get; set; }
}