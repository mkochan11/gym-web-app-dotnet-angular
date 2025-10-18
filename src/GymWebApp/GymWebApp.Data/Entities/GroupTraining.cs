using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities;

public class GroupTraining : Training
{
    [Required]
    public int TrainerId { get; set; }
    
    [ForeignKey(nameof(TrainerId))]
    public Trainer Trainer { get; set; } = null!;
    
    [Required]
    public int MaxParticipantNumber { get; set; }
    
    public List<GroupTrainingParticipation> Participations { get; set; } = [];
    
    [Required]
    public int TrainingTypeId { get; set; }
    
    [ForeignKey(nameof(TrainingTypeId))]
    public TrainingType TrainingType { get; set; } = null!;
}
