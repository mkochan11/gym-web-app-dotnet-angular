using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.IndividualTrainings;

public static class DeleteIndividualTraining
{
    public class Handler : IRequestHandler<DeleteIndividualTrainingCommand>
    {
        private readonly IIndividualTrainingRepository _repository;

        public Handler(IIndividualTrainingRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteIndividualTrainingCommand request, CancellationToken ct)
        {
            var training = await _repository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(GroupTraining), request.Id);

            if (!training.IsCancelled)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { "Training has not been cancelled." } }
                });
            }

            training!.SetRemovedTrue(
                request.UpdatedById);

            await _repository.SaveChangesAsync();

            //TODO: remove participations
        }
    }

    public class Validator : AbstractValidator<DeleteIndividualTrainingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}

public record DeleteIndividualTrainingCommand(
    int Id,
    string UpdatedById
) : IRequest;