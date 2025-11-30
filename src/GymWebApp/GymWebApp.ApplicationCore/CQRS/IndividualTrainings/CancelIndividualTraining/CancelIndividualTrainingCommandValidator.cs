using FluentValidation;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTrainings
{
    public class CancelIndividualTrainingCommandValidator : IndividualTrainingRequestValidatorBase<CancelIndividualTrainingCommand>
    {
        public CancelIndividualTrainingCommandValidator(IIndividualTrainingRepository individualTrainingRepository) : base(individualTrainingRepository)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Individual training ID must be greater than 0")
                .MustAsync(IndividualTrainingExists).WithMessage("Individual training with ID {PropertyValue} does not exist")
                .MustAsync(IndividualTrainingNotCancelled).WithMessage("Individual training with ID {PropertyValue} has already been cancelled");

            RuleFor(x => x.CancellationReason)
                .NotNull().WithMessage("Cancellation reason is required")
                .NotEmpty().WithMessage("Cancellation reason is required");
        }
    }
}