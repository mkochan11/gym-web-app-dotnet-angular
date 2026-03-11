using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using MediatR;

namespace GymWebApp.Application.CQRS.GroupTrainings;

public static class RestoreGroupTraining
{
    public class Handler : IRequestHandler<RestoreGroupTrainingCommand>
    {
        private readonly IGroupTrainingRepository _groupTrainingRepository;
        private readonly IIndividualTrainingRepository _individualTrainingRepository;
        private readonly IShiftRepository _shiftRepository;

        public Handler(
            IGroupTrainingRepository groupTrainingRepository,
            IIndividualTrainingRepository individualTrainingRepository,
            IShiftRepository shiftRepository)
        {
            _groupTrainingRepository = groupTrainingRepository;
            _individualTrainingRepository = individualTrainingRepository;
            _shiftRepository = shiftRepository;
        }

        public async Task Handle(RestoreGroupTrainingCommand command, CancellationToken cancellationToken)
        {
            var groupTraining = await _groupTrainingRepository.GetByIdAsync(command.Id) ?? throw new NotFoundException("Group training not found");

            if (!groupTraining.IsCancelled)
            {
                throw new BusinessRuleViolationException("Training has not been cancelled");
            }

            if (groupTraining.StartTime < DateTime.UtcNow)
            {
                throw new BusinessRuleViolationException("Cannot restore training that is in the past");
            }

            var hasGroupConflict = await _groupTrainingRepository.ExistsOverlappingAsync(
                groupTraining.TrainerId, groupTraining.StartTime, groupTraining.EndTime, cancellationToken);

            if (hasGroupConflict)
            {
                throw new BusinessRuleViolationException("Trainer has an overlapping group training at this time");
            }

            var hasIndividualConflict = await _individualTrainingRepository.ExistsOverlappingAsync(
                groupTraining.TrainerId, groupTraining.StartTime, groupTraining.EndTime, cancellationToken);

            if (hasIndividualConflict)
            {
                throw new BusinessRuleViolationException("Trainer has an overlapping individual training at this time");
            }

            var hasShiftConflict = await _shiftRepository.ExistsOverlappingAsync(
                groupTraining.TrainerId, groupTraining.StartTime, groupTraining.EndTime, cancellationToken);

            if (hasShiftConflict)
            {
                throw new BusinessRuleViolationException("Trainer has an overlapping shift at this time");
            }

            groupTraining.SetCancelledFalse(command.UpdatedById);
            
            //TODO: sent notification

            await _groupTrainingRepository.SaveChangesAsync();
        }
    }

    public class Validator : AbstractValidator<RestoreGroupTrainingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}

public record RestoreGroupTrainingCommand(
    int Id,
    string UpdatedById
) : IRequest;