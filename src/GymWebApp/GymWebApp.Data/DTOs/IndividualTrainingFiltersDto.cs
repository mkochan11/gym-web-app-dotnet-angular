namespace GymWebApp.Data.DTOs;

public class IndividualTrainingFiltersDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int>? TrainersIds { get; set; }
    public List<int>? ClientsIds { get; set; }
}