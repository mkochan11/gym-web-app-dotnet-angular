using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetByAccountIdAsync(string accountId);
    Task<Client?> GetByAccountIdWithDetailsAsync(string accountId);
    Task SoftDeleteByAccountIdAsync(string accountId);
    Task<Client> CreateAsync(Client client);
    Task<IEnumerable<Client>> GetAllWithMembershipAsync(string? search = null, int page = 1, int pageSize = 50);
    Task<Client?> GetByIdWithDetailsAsync(int id);
}