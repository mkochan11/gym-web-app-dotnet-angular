using GymWebApp.ApplicationCore.Extensions;
using GymWebApp.ApplicationCore.Models.Shift;
using GymWebApp.Data.DTOs;
using GymWebApp.Data.Repositories.Interfaces;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.Shift;

public class GetShiftsFilteredQueryHandler : IRequestHandler<GetShiftsFilteredQuery, IEnumerable<ShiftWebModel>>
{
    private readonly IShiftRepository _shiftRepository;

    public GetShiftsFilteredQueryHandler(
        IShiftRepository shiftRepository)
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