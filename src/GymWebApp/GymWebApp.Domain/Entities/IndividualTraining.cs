using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Domain.Entities.Abstract;

namespace GymWebApp.Domain.Entities;

public class IndividualTraining : BaseTrainingEntity
{
    [Required]
    public int TrainerId { get; set; }
    
    [ForeignKey(nameof(TrainerId))]
    public Employee Trainer { get; set; } = null!;
    
    public int? ClientId { get; set; }
    
    [ForeignKey(nameof(ClientId))]
    public Client? Client { get; set; }
}
