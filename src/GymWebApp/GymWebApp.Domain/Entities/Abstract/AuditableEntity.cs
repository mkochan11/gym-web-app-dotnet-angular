using System.ComponentModel.DataAnnotations.Schema;

namespace GymWebApp.Domain.Entities.Abstract;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string CreatedById { get; set; } = string.Empty;

    public string? UpdatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public ApplicationUser CreatedBy { get; set; } = null!;

    [ForeignKey(nameof(UpdatedById))]
    public ApplicationUser? UpdatedBy { get; set; }
}