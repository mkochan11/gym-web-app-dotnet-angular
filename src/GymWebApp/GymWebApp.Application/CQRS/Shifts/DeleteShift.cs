using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.Shifts;

public static class DeleteShift
{
    public class Handler : IRequestHandler<DeleteShiftCommand>
    {
        private readonly IShiftRepository _repository;

        public Handler(IShiftRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteShiftCommand request, CancellationToken ct)
        {
            var shift = await _repository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(GroupTraining), request.Id);

            if (!shift.IsCancelled)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { "Shift has not been cancelled." } }
                });
            }

            shift!.SetRemovedTrue(
                request.UpdatedById);

            await _repository.SaveChangesAsync();
        }
    }

    public class Validator : AbstractValidator<DeleteShiftCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}

public record DeleteShiftCommand(
    int Id,
    string UpdatedById
) : IRequest;