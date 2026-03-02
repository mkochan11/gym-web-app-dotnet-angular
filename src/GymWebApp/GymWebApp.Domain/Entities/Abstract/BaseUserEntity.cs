using GymWebApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymWebApp.Domain.Entities.Abstract;

public abstract class BaseUserEntity : BaseEntity
{
    [Required]
    [MaxLength(450)]
    public string AccountId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Surname { get; set; } = string.Empty;
    
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(200)]
    public string? Address { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    [MaxLength(10)]
    public Gender Gender { get; set; }
}
