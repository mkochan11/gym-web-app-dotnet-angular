using GymWebApp.ApplicationCore.Models.Client;
using GymWebApp.ApplicationCore.Models.Trainer;
using GymWebApp.Data.Enums;

namespace GymWebApp.ApplicationCore.Models.Training;

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