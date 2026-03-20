using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByIdWithEmploymentsAsync(int employeeId);
    Task<Employee?> GetByAccountIdAsync(string accountId);
    Task<Employee?> GetByAccountIdWithEmploymentsAsync(string accountId);
    Task SoftDeleteByAccountIdAsync(string accountId);
}