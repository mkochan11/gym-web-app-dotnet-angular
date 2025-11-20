namespace GymWebApp.ApplicationCore.Models.Training;

public class TrainingTypeWebModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? DifficultyLevel { get; set; }
}