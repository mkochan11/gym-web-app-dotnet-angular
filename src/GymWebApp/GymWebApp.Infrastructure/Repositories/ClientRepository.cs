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
}
