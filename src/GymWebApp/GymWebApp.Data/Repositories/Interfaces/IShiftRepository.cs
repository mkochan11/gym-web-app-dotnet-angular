using GymWebApp.Data.DTOs;
using GymWebApp.Data.Entities;

namespace GymWebApp.Data.Repositories.Interfaces;

public interface IShiftRepository : IRepository<Shift>
{
    Task<IEnumerable<Shift>> GetAllShiftsAsync();
    Task<IEnumerable<Shift>> GetFilteredShiftsAsync(ShiftFiltersDto filters);
}
