using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.IndividualTrainings;

public static class CancelIndividualTraining
{
    public class Handler : IRequestHandler<CancelIndividualTrainingCommand>
    {
        private readonly IIndividualTrainingRepository _repository;

        public Handler(IIndividualTrainingRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(CancelIndividualTrainingCommand request, CancellationToken ct)
        {
            var training = await _repository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(IndividualTraining), request.Id);

            if (training.IsCancelled)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { "Individual training already cancelled." } }
                });
            }

            training.SetCancelledTrue(
                request.CancellationReason,
                request.UpdatedById);

            await _repository.SaveChangesAsync();

            // TODO: send notification
            // TODO: cancel reservation if exists
        }
    }

    public class Validator : AbstractValidator<CancelIndividualTrainingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.CancellationReason)
                .NotEmpty();
        }
    }
}

public record CancelIndividualTrainingCommand(
    int Id,
    string CancellationReason,
    string UpdatedById
) : IRequest;