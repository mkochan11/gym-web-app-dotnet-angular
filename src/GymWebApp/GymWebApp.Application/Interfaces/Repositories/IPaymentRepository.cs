using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByIdWithMembershipAsync(int id);
    Task<IEnumerable<Payment>> GetPaymentsByClientIdAsync(int clientId);
}