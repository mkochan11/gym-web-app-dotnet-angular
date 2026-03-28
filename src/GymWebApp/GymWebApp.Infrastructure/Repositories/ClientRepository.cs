using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Infrastructure.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Client?> GetByAccountIdAsync(string accountId)
    {
        return await _context.Set<Client>()
            .FirstOrDefaultAsync(c => c.AccountId == accountId && !c.Removed);
    }

    public async Task<Client?> GetByAccountIdWithDetailsAsync(string accountId)
    {
        return await _context.Set<Client>()
            .Include(c => c.GymMemberships)
            .Include(c => c.IndividualTrainings)
            .Include(c => c.GroupTrainingsParticipations)
            .Include(c => c.TrainingPlans)
            .FirstOrDefaultAsync(c => c.AccountId == accountId && !c.Removed);
    }

    public async Task SoftDeleteByAccountIdAsync(string accountId)
    {
        await _context.Set<Client>()
            .Where(c => c.AccountId == accountId && !c.Removed)
            .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.Removed, true));
    }

    public async Task<Client> CreateAsync(Client client)
    {
        await _context.Set<Client>().AddAsync(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<IEnumerable<Client>> GetAllWithMembershipAsync(string? search = null, int page = 1, int pageSize = 50)
    {
        var query = _context.Set<Client>()
            .Include(c => c.GymMemberships)
            .ThenInclude(m => m.MembershipPlan)
            .Where(c => !c.Removed)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(c => 
                c.Name.ToLower().Contains(searchLower) || 
                c.Surname.ToLower().Contains(searchLower));
        }

        return await query
            .OrderBy(c => c.Surname)
            .ThenBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Client?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Set<Client>()
            .Include(c => c.GymMemberships)
            .ThenInclude(m => m.MembershipPlan)
            .FirstOrDefaultAsync(c => c.Id == id && !c.Removed);
    }
}
