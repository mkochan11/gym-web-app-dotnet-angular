using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IGymMembershipRepository : IRepository<GymMembership>
{
    Task<GymMembership?> GetByIdWithDetailsAsync(int id);
    Task<GymMembership?> GetActiveMembershipByClientIdAsync(int clientId);
    Task<IEnumerable<GymMembership>> GetMembershipsByClientIdAsync(int clientId);
}
