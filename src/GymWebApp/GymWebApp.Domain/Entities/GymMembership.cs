using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Domain.Entities.Abstract;
using GymWebApp.Domain.Enums;

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
    
    [Required]
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;
    
    public bool IsActive => Status == MembershipStatus.Active || Status == MembershipStatus.PendingCancellation;
    
    public bool IsCancelled => Status == MembershipStatus.Cancelled || Status == MembershipStatus.PendingCancellation;
    
    public DateTime? CancelledAt { get; set; }
    
    public DateTime? CancellationRequestedDate { get; set; }
    
    public DateTime? EffectiveEndDate { get; set; }
    
    [MaxLength(500)]
    public string? CancellationReason { get; set; }
}