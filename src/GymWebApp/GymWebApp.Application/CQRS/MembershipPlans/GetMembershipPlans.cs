using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.MembershipPlan;
using MediatR;

namespace GymWebApp.Application.CQRS.MembershipPlans;

public static class GetMembershipPlans
{
    public class Handler : IRequestHandler<GetMembershipPlansQuery, List<MembershipPlanWebModel>>
    {
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public Handler(IMembershipPlanRepository membershipPlanRepository)
        {
            _membershipPlanRepository = membershipPlanRepository;
        }

        public async Task<List<MembershipPlanWebModel>> Handle(GetMembershipPlansQuery query, CancellationToken ct)
        {
            var plans = await _membershipPlanRepository.GetAllAsync();
            return plans.Select(p => p.ToMembershipPlanWebModel()).ToList();
        }
    }
}

public class GetMembershipPlansQuery : IRequest<List<MembershipPlanWebModel>>
{
}
