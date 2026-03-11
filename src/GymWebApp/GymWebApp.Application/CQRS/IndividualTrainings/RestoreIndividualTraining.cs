using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using MediatR;

namespace GymWebApp.Application.CQRS.IndividualTrainings;

public static class RestoreIndividualTraining
{
    public class Handler : IRequestHandler<RestoreIndividualTrainingCommand>
    {
        private readonly IIndividualTrainingRepository _individualTrainingRepository;
        private readonly IGroupTrainingRepository _groupTrainingRepository;
        private readonly IShiftRepository _shiftRepository;

        public Handler(
            IIndividualTrainingRepository individualTrainingRepository,
            IGroupTrainingRepository groupTrainingRepository,
            IShiftRepository shiftRepository)
        {
            _individualTrainingRepository = individualTrainingRepository;
            _groupTrainingRepository = groupTrainingRepository;
            _shiftRepository = shiftRepository;
        }

        public async Task Handle(RestoreIndividualTrainingCommand command, CancellationToken cancellationToken)
        {
            var training = await _individualTrainingRepository.GetByIdAsync(command.Id) ?? throw new NotFoundException("Group training not found");

            if (!training.IsCancelled)
            {
                throw new BusinessRuleViolationException("Training has not been cancelled");
            }

            if (training.StartTime < DateTime.UtcNow)
            {
                throw new BusinessRuleViolationException("Cannot restore training that is in the past");
            }

            var hasIndividualConflict = await _individualTrainingRepository.ExistsOverlappingAsync(
                training.TrainerId, training.StartTime, training.EndTime, cancellationToken);

            if (hasIndividualConflict)
            {
                throw new BusinessRuleViolationException("Trainer has an overlapping individual training at this time");
            }

            var hasGroupConflict = await _groupTrainingRepository.ExistsOverlappingAsync(
                training.TrainerId, training.StartTime, training.EndTime, cancellationToken);

            if (hasGroupConflict)
            {
                throw new BusinessRuleViolationException("Trainer has an overlapping group training at this time");
            }

            var hasShiftConflict = await _shiftRepository.ExistsOverlappingAsync(
                training.TrainerId, training.StartTime, training.EndTime, cancellationToken);

            if (hasShiftConflict)
            {
                throw new BusinessRuleViolationException("Trainer has an overlapping shift at this time");
            }

            training.SetCancelledFalse(command.UpdatedById);
            
            //TODO: sent notification

            await _individualTrainingRepository.SaveChangesAsync();
        }
    }

    public class Validator : AbstractValidator<RestoreIndividualTrainingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}

public record RestoreIndividualTrainingCommand(
    int Id,
    string UpdatedById
) : IRequest;