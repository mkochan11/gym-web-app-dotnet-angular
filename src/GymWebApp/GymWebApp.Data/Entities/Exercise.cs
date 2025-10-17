using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Data.Entities.Abstract;


namespace GymWebApp.Data.Entities
{
    public class Exercise : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public int RepetitionsNumber { get; set; }
        
        public int SeriesNumber { get; set; }
        
        public TimeSpan RestTime { get; set; }
        
        [Required]
        public int TrainingPlanId { get; set; }
        
        [ForeignKey(nameof(TrainingPlanId))]
        public TrainingPlan TrainingPlan { get; set; } = null!;
        
        [MaxLength(100)]
        public string? MuscleGroup { get; set; }
        
        public int? Order { get; set; }
        
        [MaxLength(500)]
        public string? Instructions { get; set; }
    }
}
