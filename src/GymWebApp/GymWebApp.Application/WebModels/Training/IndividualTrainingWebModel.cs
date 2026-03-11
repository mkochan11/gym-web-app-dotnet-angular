using GymWebApp.Application.WebModels.Client;
using GymWebApp.Application.WebModels.Trainer;

namespace GymWebApp.Application.WebModels.Training;

public class IndividualTrainingWebModel
{
    public int Id { get; set; }
    
    public string Description { get; set; } = null!;
    
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public TimeSpan Duration { get; set; }

    public string[] Statuses { get; set; } = [];

    public TrainerWebModel Trainer { get; set; } = null!;

    public ClientWebModel? Client { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public bool Removed { get; set; }
}
