using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Infrastructure.Repositories;

public class GymMembershipRepository : Repository<GymMembership>, IGymMembershipRepository
{
    public GymMembershipRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<GymMembership?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Set<GymMembership>()
            .Include(m => m.Client)
            .Include(m => m.MembershipPlan)
            .Include(m => m.Payments)
            .FirstOrDefaultAsync(m => m.Id == id && !m.Removed);
    }

    public async Task<GymMembership?> GetByClientIdWithDetailsAsync(int clientId)
    {
        return await _context.Set<GymMembership>()
            .Include(m => m.Client)
            .Include(m => m.MembershipPlan)
            .Include(m => m.Payments)
            .FirstOrDefaultAsync(m => m.ClientId == clientId && !m.Removed);
    }

    public async Task<GymMembership?> GetActiveMembershipByClientIdAsync(int clientId)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Set<GymMembership>()
            .Include(m => m.Client)
            .Include(m => m.MembershipPlan)
            .FirstOrDefaultAsync(m => 
                m.ClientId == clientId && 
                !m.Removed && 
                (m.Status == MembershipStatus.Active || m.Status == MembershipStatus.PendingCancellation) &&
                m.EndDate >= today);
    }

    public async Task<IEnumerable<GymMembership>> GetMembershipsByClientIdAsync(int clientId)
    {
        return await _context.Set<GymMembership>()
            .Include(m => m.MembershipPlan)
            .Where(m => m.ClientId == clientId && !m.Removed)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasActiveMembershipAsync(int clientId)
    {
        var today = DateTime.UtcNow.Date;
        var hasActive = await _context.Set<GymMembership>()
            .AnyAsync(m => 
                m.ClientId == clientId && 
                !m.Removed && 
                m.Status == MembershipStatus.Active &&
                m.EndDate >= today);
        return hasActive;
    }

    public async Task<IEnumerable<GymMembership>> GetPendingCancellationsAsync(DateTime effectiveEndDate, CancellationToken ct = default)
    {
        return await _context.Set<GymMembership>()
            .Include(m => m.Payments)
            .Where(m => 
                !m.Removed && 
                m.Status == MembershipStatus.PendingCancellation &&
                m.EffectiveEndDate <= effectiveEndDate)
            .ToListAsync(ct);
    }
}
