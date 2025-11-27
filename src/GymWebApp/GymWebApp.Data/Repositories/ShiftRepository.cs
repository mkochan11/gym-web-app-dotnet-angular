using GymWebApp.Data.DTOs;
using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Data.Repositories;

public class ShiftRepository : Repository<Shift>, IShiftRepository
{
    public ShiftRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Shift>> GetAllShiftsAsync() => 
        await _context.Shifts
            .Include(s => s.Employee)
            .Where(s => !s.Removed)
            .ToListAsync();

    public async Task<IEnumerable<Shift>> GetFilteredShiftsAsync(ShiftFiltersDto filters)
    {
        var query = _context.Shifts
            .Include(s => s.Employee)
            .Where(s => !s.Removed && !s.IsCancelled)
            .AsQueryable();

        if (filters.StartDate.HasValue)
        {
            query = query.Where(s => s.StartTime >= filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(s => s.EndTime <= filters.EndDate.Value);
        }

        if (filters.EmployeeIds?.Any() == true)
        {
            query = query.Where(s => filters.EmployeeIds.Contains(s.EmployeeId));
        }

        return await query.ToListAsync();
    }
}