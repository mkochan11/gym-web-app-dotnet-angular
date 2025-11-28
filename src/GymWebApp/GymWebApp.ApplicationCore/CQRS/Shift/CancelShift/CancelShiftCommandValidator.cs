using FluentValidation;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.ApplicationCore.CQRS.Shift
{
    public class CancelShiftCommandValidator : ShiftRequestValidatorBase<CancelShiftCommand>
    {
        public CancelShiftCommandValidator(IShiftRepository shiftRepository) : base(shiftRepository)
        {

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Shift ID must be greater than 0")
                .MustAsync(ShiftExists).WithMessage("Shift with ID {PropertyValue} does not exist")
                .MustAsync(ShiftNotCancelled).WithMessage("Shift with ID {PropertyValue} has already been cancelled");

            RuleFor(x => x.CancellationReason)
                .NotNull().WithMessage("Cancellation reason is required")
                .NotEmpty().WithMessage("Cancellation reason is required");
        }
    }
}