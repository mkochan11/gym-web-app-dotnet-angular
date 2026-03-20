using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.MembershipPlan;
using MediatR;

namespace GymWebApp.Application.CQRS.MembershipPlans;

public static class GetMembershipPlanById
{
    public class Handler : IRequestHandler<GetMembershipPlanByIdQuery, MembershipPlanWebModel?>
    {
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public Handler(IMembershipPlanRepository membershipPlanRepository)
        {
            _membershipPlanRepository = membershipPlanRepository;
        }

        public async Task<MembershipPlanWebModel?> Handle(GetMembershipPlanByIdQuery query, CancellationToken ct)
        {
            var plan = await _membershipPlanRepository.GetByIdAsync(query.Id);
            return plan?.ToMembershipPlanWebModel();
        }
    }
}

public class GetMembershipPlanByIdQuery : IRequest<MembershipPlanWebModel?>
{
    public int Id { get; set; }
}
