using Microsoft.EntityFrameworkCore;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByIdWithEmploymentsAsync(int employeeId)
    {
        return await _context.Set<Employee>()
                       .Include(e => e.Employments)
                       .FirstOrDefaultAsync(e => e.Id == employeeId);
    }

    public async Task<Employee?> GetByAccountIdAsync(string accountId)
    {
        return await _context.Set<Employee>()
            .FirstOrDefaultAsync(e => e.AccountId == accountId && !e.Removed);
    }

    public async Task<Employee?> GetByAccountIdWithEmploymentsAsync(string accountId)
    {
        return await _context.Set<Employee>()
            .Include(e => e.Employments)
            .FirstOrDefaultAsync(e => e.AccountId == accountId && !e.Removed);
    }

    public async Task SoftDeleteByAccountIdAsync(string accountId)
    {
        await _context.Set<Employee>()
            .Where(e => e.AccountId == accountId && !e.Removed)
            .ExecuteUpdateAsync(setters => setters.SetProperty(e => e.Removed, true));
    }
}
