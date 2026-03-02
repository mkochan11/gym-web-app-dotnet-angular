using GymWebApp.Domain.Entities.Abstract;

namespace GymWebApp.Application.Extensions;

public static class AuditableEntityExtensions
{
    public static void SetCreationAuditInfo(this AuditableEntity entity, string userId)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedById = userId;
        }

    public static void SetModificationAuditInfo(this AuditableEntity entity, string userId)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedById = userId;
    }
}
