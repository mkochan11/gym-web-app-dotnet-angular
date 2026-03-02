using GymWebApp.Application.DTOs;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Shift;
using MediatR;

namespace GymWebApp.Application.CQRS.Shifts;

public static class GetShiftsFiltered
{
    public class Handler : IRequestHandler<GetShiftsFilteredQuery, IEnumerable<ShiftWebModel>>
    {
        private readonly IShiftRepository _shiftRepository;

        public Handler(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task<IEnumerable<ShiftWebModel>> Handle(GetShiftsFilteredQuery request, CancellationToken cancellationToken)
        {
            var filtersDto = new ShiftFiltersDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                EmployeeIds = request.EmployeeIds.ToIntList()
            };
            
            var shifts = await _shiftRepository.GetFilteredShiftsAsync(filtersDto);
            return shifts.Select(s => s.ToShiftWebModel()).ToList();
        }
    }
}

public record GetShiftsFilteredQuery(
    DateTime? StartDate,
    DateTime? EndDate,
    string? EmployeeIds
) : IRequest<IEnumerable<ShiftWebModel>>;