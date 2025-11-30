using FluentValidation;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTrainings
{
    public class CreateIndividualTrainingCommandValidator : AbstractValidator<CreateIndividualTrainingCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public CreateIndividualTrainingCommandValidator(
            IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;

            RuleFor(x => x.TrainerId)
                .GreaterThan(0)
                .MustAsync(TrainerExists)
                .WithMessage("Trainer with ID {PropertyValue} does not exist");

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
    }
}