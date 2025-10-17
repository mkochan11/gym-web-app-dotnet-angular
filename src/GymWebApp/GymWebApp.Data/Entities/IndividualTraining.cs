using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities
{
    public class IndividualTraining : Training
    {
        [Required]
        public int TrainerId { get; set; }
        
        [ForeignKey(nameof(TrainerId))]
        public Trainer Trainer { get; set; } = null!;
        
        public int? ClientId { get; set; }
        
        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }
    }
}
