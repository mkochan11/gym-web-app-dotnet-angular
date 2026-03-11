using GymWebApp.Application.DTOs;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Infrastructure.Repositories;

public class ShiftRepository : Repository<Shift>, IShiftRepository
{
    public ShiftRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsOverlappingAsync(int employeeId, DateTime start, DateTime end, CancellationToken ct)
    {
        return await _context.Shifts
            .AnyAsync(s => 
                s.EmployeeId == employeeId && 
                !s.Removed &&
                s.StartTime < end && 
                s.EndTime > start, 
                ct);
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
            .Where(s => !s.Removed)
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