using FluentValidation;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.ApplicationCore.CQRS.Shifts
{
    public class ShiftRequestValidatorBase<T> : AbstractValidator<T> where T : class
    {
        protected readonly IShiftRepository _shiftRepository;

        public ShiftRequestValidatorBase(
            IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        protected async Task<bool> ShiftExists(int id, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            return shift != null;
        }

        protected async Task<bool> ShiftNotCancelled(int id, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            return shift?.IsCancelled != true;
        }

        protected async Task<bool> ShiftNotStarted(int id, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            return shift?.StartTime > DateTime.UtcNow;
        }
    }
}