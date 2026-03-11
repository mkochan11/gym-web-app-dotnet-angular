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

        public Handler(IIndividualTrainingRepository individualTrainingRepository)
        {
            _individualTrainingRepository = individualTrainingRepository;
        }

        public async Task Handle(RestoreIndividualTrainingCommand command, CancellationToken cancellationToken)
        {
            var training = await _individualTrainingRepository.GetByIdAsync(command.Id) ?? throw new NotFoundException("Group training not found");

            if (!training.IsCancelled)
            {
                throw new BusinessRuleViolationException("Training has not been cancelled");
            }

            training.SetCancelledFalse(command.UpdatedById);

            //TODO: check if training is in the past and if so, don't allow to restore it
            //TODO: check if trainer is available at the time of training, if not, don't allow to restore it

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