using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using MediatR;

namespace GymWebApp.Application.CQRS.Shifts;

public static class RestoreShift
{
    public class Handler : IRequestHandler<RestoreShiftCommand>
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly ITrainerService _trainerService;

        public Handler(IShiftRepository shiftRepository, ITrainerService trainerService)
        {
            _shiftRepository = shiftRepository;
            _trainerService = trainerService;
        }

        public async Task Handle(RestoreShiftCommand command, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.GetByIdAsync(command.Id) ?? throw new NotFoundException("Shift not found");

            if (!shift.IsCancelled)
            {
                throw new BusinessRuleViolationException("Shift has not been cancelled");
            }

            if (shift.StartTime < DateTime.UtcNow)
            {
                throw new BusinessRuleViolationException("Cannot restore shift that is in the past");
            }

            var hasShiftConflict = await _shiftRepository.ExistsOverlappingAsync(
                shift.EmployeeId, shift.StartTime, shift.EndTime, cancellationToken);

            if (hasShiftConflict)
            {
                throw new BusinessRuleViolationException("Employee has an overlapping shift at this time");
            }

            var isTrainerAvailable = await _trainerService.IsAvailableAsync(
                shift.EmployeeId, shift.StartTime, shift.EndTime, cancellationToken);

            if (!isTrainerAvailable)
            {
                throw new BusinessRuleViolationException("Employee has an overlapping training at this time");
            }

            shift.SetCancelledFalse(command.UpdatedById);
            
            //TODO: sent notification

            await _shiftRepository.SaveChangesAsync();
        }
    }

    public class Validator : AbstractValidator<RestoreShiftCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}

public record RestoreShiftCommand(
    int Id,
    string UpdatedById
) : IRequest;
