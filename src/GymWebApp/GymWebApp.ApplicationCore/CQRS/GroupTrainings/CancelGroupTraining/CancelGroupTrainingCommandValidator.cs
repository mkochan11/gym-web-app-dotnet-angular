using FluentValidation;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.ApplicationCore.CQRS.GroupTrainings;

public class CancelGroupTrainingCommandValidator : GroupTrainingRequestValidatorBase<CancelGroupTrainingCommand>
{
    public CancelGroupTrainingCommandValidator(IGroupTrainingRepository groupTrainingRepository) : base(groupTrainingRepository)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Group training ID must be greater than 0")
            .MustAsync(GroupTrainingExists).WithMessage("Group training with ID {PropertyValue} does not exist")
            .MustAsync(GroupTrainingNotCancelled).WithMessage("Group training with ID {PropertyValue} has already been cancelled");

        RuleFor(x => x.CancellationReason)
            .NotNull().WithMessage("Cancellation reason is required")
            .NotEmpty().WithMessage("Cancellation reason is required");
    }
}