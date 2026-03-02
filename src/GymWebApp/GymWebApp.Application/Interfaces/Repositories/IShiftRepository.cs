using GymWebApp.Application.DTOs;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IShiftRepository : IRepository<Shift>
{
    Task<IEnumerable<Shift>> GetAllShiftsAsync();
    Task<IEnumerable<Shift>> GetFilteredShiftsAsync(ShiftFiltersDto filters);
}
