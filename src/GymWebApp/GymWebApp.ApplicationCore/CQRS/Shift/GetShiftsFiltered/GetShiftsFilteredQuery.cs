using GymWebApp.ApplicationCore.Models.Shift;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.Shift;

public class GetShiftsFilteredQuery : IRequest<IEnumerable<ShiftWebModel>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? EmployeeIds { get; set; }
}