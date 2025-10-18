using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities;

public class Shift : AuditableEntity
{
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [Required]
    public int EmployeeId { get; set; }
    
    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;
}
