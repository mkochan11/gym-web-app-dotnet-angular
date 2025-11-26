using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.Data.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }
}