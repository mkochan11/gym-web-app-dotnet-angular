using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.GroupTrainings;

public static class CancelGroupTraining
{
    public class Handler : IRequestHandler<CancelGroupCommand>
    {
        private readonly IGroupTrainingRepository _repository;

        public Handler(IGroupTrainingRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(CancelGroupCommand request, CancellationToken ct)
        {
            var training = await _repository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(GroupTraining), request.Id);

            if (training.IsCancelled)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { "Training already cancelled." } }
                });
            }

            training!.SetCancelledTrue(
                request.CancellationReason,
                request.UpdatedById);

            await _repository.SaveChangesAsync();


            //TODO: send notification
            //TODO: cancel participations
        }
    }

    public class Validator : AbstractValidator<CancelGroupCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.CancellationReason).NotEmpty();
        }
    }
}

public record CancelGroupCommand(
    int Id,
    string CancellationReason,
    string UpdatedById
) : IRequest;