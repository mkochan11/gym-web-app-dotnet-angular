using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Domain.Entities.Abstract;

namespace GymWebApp.Domain.Entities;

public class Shift : AuditableEntity
{
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [Required]
    public int EmployeeId { get; set; }
    
    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public bool IsCancelled { get; set; } = false;

    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    public DateTime? CancelledAt { get; set; }
}
