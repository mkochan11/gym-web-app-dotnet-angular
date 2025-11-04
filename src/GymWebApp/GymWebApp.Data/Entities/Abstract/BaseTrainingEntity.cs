using System.ComponentModel.DataAnnotations;

namespace GymWebApp.Data.Entities.Abstract;

public abstract class BaseTrainingEntity : AuditableEntity
{
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public TimeSpan Duration { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public bool IsCompleted { get; set; } = false;
    
    public bool IsCancelled { get; set; } = false;
    
    [MaxLength(500)]
    public string? CancellationReason { get; set; }
    
    public DateTime? CancelledAt { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
}