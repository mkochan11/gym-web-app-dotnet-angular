using FluentValidation;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.ApplicationCore.CQRS.GroupTrainings
{
    public class CreateGroupTrainingCommandValidator : AbstractValidator<CreateGroupTrainingCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITrainingTypeRepository _trainingTypeRepository;

        public CreateGroupTrainingCommandValidator(
            IEmployeeRepository employeeRepository,
            ITrainingTypeRepository trainingTypeRepository)
        {
            _employeeRepository = employeeRepository;
            _trainingTypeRepository = trainingTypeRepository;

            RuleFor(x => x.TrainerId)
                .GreaterThan(0)
                .MustAsync(TrainerExists)
                .WithMessage("Trainer with ID {PropertyValue} does not exist");

            RuleFor(x => x.TrainingTypeId)
                .GreaterThan(0)
                .MustAsync(TrainingTypeExists)
                .WithMessage("Training type with ID {PropertyValue} does not exist");

            RuleFor(x => x.MaxParticipantNumber)
                .GreaterThan(0)
                .WithMessage("Max participants must be greater than 0");

            RuleFor(x => x.DifficultyLevel)
                .InclusiveBetween(1, 5)
                .WithMessage("Difficulty level must be between 1 and 5");

            RuleFor(x => x.StartDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Training date must be in the future");

            RuleFor(x => TimeSpan.FromMinutes((x.EndDate - x.StartDate).TotalMinutes))
                .Must(d => d.TotalMinutes >= 15 && d.TotalMinutes <= 180)
                .WithMessage("Duration must be between 15 and 180 minutes");

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.Notes)
                .MaximumLength(1000);
        }

        private async Task<bool> TrainerExists(int trainerId, CancellationToken cancellationToken)
        {
            var trainer = await _employeeRepository.GetByIdAsync(trainerId);
            return (trainer != null && trainer.Role == Data.Enums.EmployeeRole.Trainer);
        }

        private async Task<bool> TrainingTypeExists(int trainingTypeId, CancellationToken cancellationToken)
        {
            var type = await _trainingTypeRepository.GetByIdAsync(trainingTypeId);
            return type != null;
        }
    }
}