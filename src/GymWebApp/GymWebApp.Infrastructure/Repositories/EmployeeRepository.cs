using System.Threading.Tasks;
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
}