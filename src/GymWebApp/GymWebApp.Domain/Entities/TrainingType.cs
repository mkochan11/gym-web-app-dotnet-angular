using System.ComponentModel.DataAnnotations;
using GymWebApp.Domain.Entities.Abstract;

namespace GymWebApp.Domain.Entities;

public class TrainingType : AuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public int? DifficultyLevel { get; set; }
    
    public bool IsActive { get; set; } = true;
}
