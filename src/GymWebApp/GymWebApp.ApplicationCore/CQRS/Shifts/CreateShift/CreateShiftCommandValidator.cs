using FluentValidation;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.ApplicationCore.CQRS.Shifts
{
    public class CreateShiftCommandValidator : AbstractValidator<CreateShiftCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public CreateShiftCommandValidator(
            IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;

            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .MustAsync(ReceptionistExists)
                .WithMessage("Receptionst with ID {PropertyValue} does not exist");

            RuleFor(x => x.StartTime)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Shift must start in the future");

            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime)
                .WithMessage("Start time must be before end time");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("End time must be after start time");
        }

        private async Task<bool> ReceptionistExists(int trainerId, CancellationToken cancellationToken)
        {
            var trainer = await _employeeRepository.GetByIdAsync(trainerId);
            return (trainer != null && trainer.Role == Data.Enums.EmployeeRole.Receptionist);
        }
    }
}