using MediatR;
using System.ComponentModel.DataAnnotations;

namespace GymWebApp.ApplicationCore.CQRS.GroupTrainings
{
    public class CreateGroupTrainingCommand : IRequest<int>
    {
        [Required]
        public int TrainerId { get; set; }

        [Required]
        public int MaxParticipantNumber { get; set; }

        [Required]
        public int TrainingTypeId { get; set; }

        [Required]
        public int DifficultyLevel { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Description { get; set; }
        public string? Notes { get; set; }

        public string CreatedById { get; set; } = string.Empty;
    }
}