using MediatR;
using System.ComponentModel.DataAnnotations;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTrainings
{
    public class CreateIndividualTrainingCommand : IRequest<int>
    {
        [Required]
        public int TrainerId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Description { get; set; }

        public string Notes { get; set; } = string.Empty;

        public string CreatedById { get; set; } = string.Empty;
    }
}