using GymWebApp.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Data.Entities.Abstract;


namespace GymWebApp.Data.Entities;

public class Payment : AuditableEntity
{
    [Required]
    public DateTime PaymentDate { get; set; }
    
    [Required]
    public int GymMembershipId { get; set; }
    
    [ForeignKey(nameof(GymMembershipId))]
    public GymMembership GymMembership { get; set; } = null!;
    
    [Required]
    public PaymentMethod PaymentMethod { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(4, 2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(100)]
    public string? TransactionId { get; set; }
    
    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }
    
    public bool IsSuccessful { get; set; } = true;
    
    [MaxLength(500)]
    public string? FailureReason { get; set; }
    
    public DateTime? ProcessedAt { get; set; }
    
    [MaxLength(200)]
    public string? ProcessedBy { get; set; }
}
