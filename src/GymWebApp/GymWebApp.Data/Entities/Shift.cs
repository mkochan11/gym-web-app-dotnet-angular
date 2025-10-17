using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities
{
    public class Shift : BaseEntity
    {
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        [Required]
        public int ReceptionistId { get; set; }
        
        [ForeignKey(nameof(ReceptionistId))]
        public Receptionist Receptionist { get; set; } = null!;
    }
}
