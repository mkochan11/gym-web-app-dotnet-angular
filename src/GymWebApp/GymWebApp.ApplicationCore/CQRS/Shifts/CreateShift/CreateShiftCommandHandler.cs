using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories.Interfaces;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.Shifts
{
    public class CreateShiftCommandHandler : IRequestHandler<CreateShiftCommand, int>
    {
        private readonly IShiftRepository _shiftRepository;

        public CreateShiftCommandHandler(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task<int> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = new Shift
            {
                EmployeeId = request.EmployeeId,
                StartTime = request.StartTime.ToUniversalTime(),
                EndTime = request.EndTime.ToUniversalTime(),
                CreatedById = request.CreatedById,
                CreatedAt = DateTime.UtcNow
            };

            await _shiftRepository.AddAsync(shift);
            await _shiftRepository.SaveChangesAsync();

            return shift.Id;
        }
    }
}