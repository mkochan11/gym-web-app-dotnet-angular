using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Data.Entities.Abstract;


namespace GymWebApp.Data.Entities;

public class MembershipPlan : AuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(4, 2)")]
    public decimal Price { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string DurationTime { get; set; } = string.Empty;
    
    [Required]
    public int DurationInMonths { get; set; }
    
    public bool CanReserveTrainings { get; set; }
    
    public bool CanAccessGroupTraining { get; set; }
    
    public bool CanAccessPersonalTraining { get; set; }

    public bool CanReceiveTrainingPlans { get; set; }
    
    public int? MaxTrainingsPerMonth { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public List<GymMembership> GymMemberships { get; set; } = [];
}
