using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities;

public class TrainingPlan : AuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public List<TrainingPlanSession> Sessions { get; set; } = [];
    
    [Required]
    public int ClientId { get; set; }
    
    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; } = null!;
    
    [Required]
    public int TrainerId { get; set; }
    
    [ForeignKey(nameof(TrainerId))]
    public Employee Trainer { get; set; } = null!;
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public bool IsActive { get; set; } = true;
}
