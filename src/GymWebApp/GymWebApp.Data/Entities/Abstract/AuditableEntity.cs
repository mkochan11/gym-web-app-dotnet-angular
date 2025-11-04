using System.ComponentModel.DataAnnotations.Schema;

namespace GymWebApp.Data.Entities.Abstract;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int CreatedById { get; set; }
    
    public int? UpdatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public BaseUserEntity CreatedBy { get; set; } = null!;

    [ForeignKey(nameof(UpdatedById))]
    public BaseUserEntity? UpdatedBy { get; set; }
}