namespace GymWebApp.Data.DTOs;

public class GroupTrainingFiltersDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int>? TrainerIds { get; set; }
    public List<int>? TrainingTypeIds { get; set; }
}