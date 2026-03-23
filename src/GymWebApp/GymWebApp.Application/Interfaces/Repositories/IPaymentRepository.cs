using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByIdWithMembershipAsync(int id);
    Task<IEnumerable<Payment>> GetPaymentsByClientIdAsync(int clientId);
    Task<IEnumerable<Payment>> GetPaymentsByMembershipIdAsync(int membershipId);
    Task<IEnumerable<Payment>> GetFuturePendingPaymentsAsync(int membershipId, DateTime cutoffDate);
    Task CancelFuturePaymentsAsync(int membershipId, DateTime cutoffDate, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Payment> payments, CancellationToken ct = default);
}