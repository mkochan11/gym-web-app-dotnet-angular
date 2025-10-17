using GymWebApp.Data.Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymWebApp.Data.Entities
{
    public class Employment : BaseEntity
    {
        [Required]
        public int EmployeeId { get; set; }
        
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Salary { get; set; }
        
        public bool IsActive => EndDate < DateTime.UtcNow;
        
        public TimeSpan? Duration => EndDate.HasValue ? EndDate.Value - StartDate : DateTime.UtcNow - StartDate;
        
        public int? DurationInDays => (int?)Duration?.TotalDays;
        
        public int? DurationInMonths => DurationInDays.HasValue ? DurationInDays.Value / 30 : null;
    }
}
