using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByIdWithMembershipAsync(int id)
    {
        return await _context.Set<Payment>()
            .Include(p => p.GymMembership)
                .ThenInclude(m => m!.MembershipPlan)
            .Include(p => p.GymMembership)
                .ThenInclude(m => m!.Client)
            .FirstOrDefaultAsync(p => p.Id == id && !p.Removed);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByClientIdAsync(int clientId)
    {
        return await _context.Set<Payment>()
            .Include(p => p.GymMembership)
                .ThenInclude(m => m!.MembershipPlan)
            .Where(p => p.GymMembership.ClientId == clientId && !p.Removed)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
}