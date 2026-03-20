using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Infrastructure.Repositories;

public class MembershipPlanRepository : Repository<MembershipPlan>, IMembershipPlanRepository
{
    public MembershipPlanRepository(ApplicationDbContext context) : base(context)
    {
    }
}
