using GymWebApp.ApplicationCore.Models.Trainer;

namespace GymWebApp.ApplicationCore.Models.Training;

public class GroupTrainingWebModel
{
    public int Id { get; set; }
    
    public string Description { get; set; } = null!;
    
    public DateTime Date { get; set; }

    public TimeSpan Duration { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsCancelled { get; set; }

    public TrainerWebModel Trainer { get; set; } = null!;

    public int MaxParticipantNumber { get; set; }

    public int CurrentParticipantNumber { get; set; }

    public TrainingTypeWebModel TrainingType { get; set; } = null!;

    public int DifficultyLevel { get; set; }
}
