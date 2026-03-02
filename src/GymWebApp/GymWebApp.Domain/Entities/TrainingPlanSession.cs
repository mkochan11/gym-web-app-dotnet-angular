using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Domain.Entities.Abstract;

namespace GymWebApp.Domain.Entities;

public class TrainingPlanSession : AuditableEntity
{
    public List<Exercise> Exercises { get; set; } = [];

    public int TrainingPlanId { get; set; }

    [ForeignKey(nameof(TrainingPlanId))]
    public TrainingPlan TrainingPlan { get; set; } = null!;
    
    public DateTime SessionDate { get; set; }
    
    public string Notes { get; set; } = string.Empty;
}