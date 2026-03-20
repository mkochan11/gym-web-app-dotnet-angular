using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Domain.Entities.Abstract;

namespace GymWebApp.Domain.Entities;

public class GymMembership : AuditableEntity
{
    [Required]
    public int MembershipPlanId { get; set; }
    
    [ForeignKey(nameof(MembershipPlanId))]
    public MembershipPlan MembershipPlan { get; set; } = null!;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public int ClientId { get; set; }
    
    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; } = null!;
    
    public List<Payment> Payments { get; set; } = [];
    
    public bool IsActive { get; set; } = true;
    
    public bool IsCancelled { get; set; } = false;
    
    public DateTime? CancelledAt { get; set; }
    
    [MaxLength(500)]
    public string? CancellationReason { get; set; }
}