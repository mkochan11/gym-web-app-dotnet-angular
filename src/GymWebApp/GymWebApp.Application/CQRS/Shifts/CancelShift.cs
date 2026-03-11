using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using MediatR;

namespace GymWebApp.Application.CQRS.Shifts;

public static class CancelShift
{
    public class Handler : IRequestHandler<CancelShiftCommand>
    {
        private readonly IShiftRepository _shiftRepository;

        public Handler(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task Handle(CancelShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(Shift), request.Id);

            if (shift.IsCancelled)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { "Shift already cancelled." } }
                });
            }

            shift.SetCancelledTrue(
                request.CancellationReason,
                request.UpdatedById);

            await _shiftRepository.SaveChangesAsync();

            // TODO: send notification
        }
    }
}

public record CancelShiftCommand(
    int Id,
    string CancellationReason,
    string UpdatedById
) : IRequest;