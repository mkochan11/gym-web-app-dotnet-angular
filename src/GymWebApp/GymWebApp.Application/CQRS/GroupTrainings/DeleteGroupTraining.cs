using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.GroupTrainings;

public static class DeleteGroupTraining
{
    public class Handler : IRequestHandler<DeleteGroupTrainingCommand>
    {
        private readonly IGroupTrainingRepository _repository;

        public Handler(IGroupTrainingRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteGroupTrainingCommand request, CancellationToken ct)
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

    public class Validator : AbstractValidator<DeleteGroupTrainingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}

public record DeleteGroupTrainingCommand(
    int Id,
    string UpdatedById
) : IRequest;