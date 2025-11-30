using MediatR;
using System.ComponentModel.DataAnnotations;

namespace GymWebApp.ApplicationCore.CQRS.Shifts
{
    public class CreateShiftCommand : IRequest<int>
    {
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string CreatedById { get; set; } = string.Empty;
    }
}