using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
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
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByMembershipIdAsync(int membershipId)
    {
        return await _context.Set<Payment>()
            .Where(p => p.GymMembershipId == membershipId && !p.Removed)
            .OrderBy(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetFuturePendingPaymentsAsync(int membershipId, DateTime cutoffDate)
    {
        return await _context.Set<Payment>()
            .Where(p => 
                p.GymMembershipId == membershipId && 
                !p.Removed &&
                p.Status == PaymentStatus.Pending &&
                p.DueDate > cutoffDate)
            .ToListAsync();
    }

    public async Task CancelFuturePaymentsAsync(int membershipId, DateTime cutoffDate, CancellationToken ct = default)
    {
        var payments = await _context.Set<Payment>()
            .Where(p => 
                p.GymMembershipId == membershipId && 
                !p.Removed &&
                p.Status == PaymentStatus.Pending &&
                p.DueDate > cutoffDate)
            .ToListAsync(ct);

        foreach (var payment in payments)
        {
            payment.Status = PaymentStatus.Cancelled;
        }
        
        await SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Payment> payments, CancellationToken ct = default)
    {
        await _context.Set<Payment>().AddRangeAsync(payments, ct);
    }
}