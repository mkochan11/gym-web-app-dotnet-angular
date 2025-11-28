using GymWebApp.Data.Repositories.Interfaces;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.Shift
{
    public class CancelShiftCommandHandler : IRequestHandler<CancelShiftCommand>
    {
        private readonly IShiftRepository _shiftRepository;

        public CancelShiftCommandHandler(
            IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task Handle(CancelShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.GetByIdAsync(request.Id);

            shift!.IsCancelled = true;
            shift!.CancellationReason = request.CancellationReason;

            //TODO: sent notification

            await _shiftRepository.SaveChangesAsync();
        }
    }
}