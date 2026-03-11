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

        public Handler(IGroupTrainingRepository groupTrainingRepository)
        {
            _groupTrainingRepository = groupTrainingRepository;
        }

        public async Task Handle(RestoreGroupTrainingCommand command, CancellationToken cancellationToken)
        {
            var groupTraining = await _groupTrainingRepository.GetByIdAsync(command.Id) ?? throw new NotFoundException("Group training not found");

            if (!groupTraining.IsCancelled)
            {
                throw new BusinessRuleViolationException("Training has not been cancelled");
            }

            groupTraining.SetCancelledFalse(command.UpdatedById);

            //TODO: check if training is in the past and if so, don't allow to restore it
            //TODO: check if trainer is available at the time of training, if not, don't allow to restore it

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