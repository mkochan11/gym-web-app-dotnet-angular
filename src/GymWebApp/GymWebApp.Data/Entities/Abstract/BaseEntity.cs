using System.ComponentModel.DataAnnotations;

namespace GymWebApp.Data.Entities.Abstract
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }

        public bool Removed { get; set; } = false;
    }
}
