using System.ComponentModel.DataAnnotations;

namespace GymWebApp.Domain.Entities.Abstract;

public abstract class BaseTrainingEntity : AuditableEntity
{
    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public bool IsCancelled { get; set; } = false;
    
    [MaxLength(500)]
    public string? CancellationReason { get; set; }
    
    public DateTime? CancelledAt { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
}