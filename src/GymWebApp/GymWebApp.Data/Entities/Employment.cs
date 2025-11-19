using GymWebApp.Data.Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymWebApp.Data.Entities;

public class Employment : AuditableEntity
{
    [Required]
    public int EmployeeId { get; set; }
    
    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? HourlyRate { get; set; }
}
