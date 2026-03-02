using System.ComponentModel.DataAnnotations;

namespace GymWebApp.Domain.Entities.Abstract;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
    
    public bool Removed { get; set; } = false;
}