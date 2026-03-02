using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Domain.Entities.Abstract;

namespace GymWebApp.Domain.Entities;

public class GroupTrainingParticipation : AuditableEntity
{
    [Required]
    public int GroupTrainingId { get; set; }
    
    [ForeignKey(nameof(GroupTrainingId))]
    public GroupTraining GroupTraining { get; set; } = null!;
    
    [Required]
    public int ClientId { get; set; }
    
    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; } = null!;
    
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    
    public bool IsCancelled { get; set; } = false;
    
    public DateTime? CancelledAt { get; set; }
}
