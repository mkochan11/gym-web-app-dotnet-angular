using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IGymMembershipRepository : IRepository<GymMembership>
{
    Task<GymMembership?> GetByIdWithDetailsAsync(int id);
    Task<GymMembership?> GetActiveMembershipByClientIdAsync(int clientId);
    Task<IEnumerable<GymMembership>> GetMembershipsByClientIdAsync(int clientId);
    Task<bool> HasActiveMembershipAsync(int clientId);
    Task<IEnumerable<GymMembership>> GetPendingCancellationsAsync(DateTime effectiveEndDate, CancellationToken ct = default);
}
