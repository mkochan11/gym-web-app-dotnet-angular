using System.ComponentModel.DataAnnotations.Schema;

namespace GymWebApp.Data.Entities.Abstract;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int CreatedById { get; set; }
    
    public int? UpdatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public User CreatedBy { get; set; }

    [ForeignKey(nameof(UpdatedById))]
    public User? UpdatedBy { get; set; }
}