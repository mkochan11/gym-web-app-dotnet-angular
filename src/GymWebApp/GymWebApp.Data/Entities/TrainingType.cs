using System.ComponentModel.DataAnnotations;
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities
{
    public class TrainingType : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public int? DifficultyLevel { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
