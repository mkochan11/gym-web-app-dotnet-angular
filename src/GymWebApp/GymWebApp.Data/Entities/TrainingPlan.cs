using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities
{
    public class TrainingPlan : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
        
        [Required]
        public int ClientId { get; set; }
        
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;
        
        [Required]
        public int PersonalTrainerId { get; set; }
        
        [ForeignKey(nameof(PersonalTrainerId))]
        public PersonalTrainer PersonalTrainer { get; set; } = null!;
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
