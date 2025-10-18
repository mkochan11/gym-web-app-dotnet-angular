using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities;

public class Exercise : AuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public int RepetitionsNumber { get; set; }
    
    public int SeriesNumber { get; set; }
    
    public TimeSpan RestTime { get; set; }
    
    [Required]
    public int TrainingPlanSessionId { get; set; }
    
    [ForeignKey(nameof(TrainingPlanSessionId))]
    public TrainingPlanSession TrainingPlanSession { get; set; } = null!;
    
    [MaxLength(100)]
    public string? MuscleGroup { get; set; }
    
    public int? Order { get; set; }
    
    [MaxLength(500)]
    public string? Instructions { get; set; }
}
