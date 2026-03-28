using FluentValidation;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.MembershipPlan;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class GetAvailablePlans
{
    public class Query : IRequest<List<MembershipPlanWebModel>>
    {
        public int MembershipId { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<MembershipPlanWebModel>>
    {
        private readonly IGymMembershipRepository _gymMembershipRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public Handler(IGymMembershipRepository gymMembershipRepository, IMembershipPlanRepository membershipPlanRepository)
        {
            _gymMembershipRepository = gymMembershipRepository;
            _membershipPlanRepository = membershipPlanRepository;
        }

        public async Task<List<MembershipPlanWebModel>> Handle(Query query, CancellationToken ct)
        {
            var membership = await _gymMembershipRepository.GetByIdWithDetailsAsync(query.MembershipId);
            
            if (membership == null)
            {
                throw new GymWebApp.Application.Common.Exceptions.NotFoundException("GymMembership", query.MembershipId);
            }

            if (membership.Status != MembershipStatus.Active)
            {
                throw new GymWebApp.Application.Common.Exceptions.NotActiveMembershipException(
                    $"Cannot change plan. Membership is not active. Current status: {membership.Status}");
            }

            var allPlans = await _membershipPlanRepository.GetAllAsync();
            var availablePlans = allPlans
                .Where(p => p.IsActive && p.Id != membership.MembershipPlanId)
                .Select(p => p.ToMembershipPlanWebModel())
                .ToList();

            return availablePlans;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.MembershipId)
                .GreaterThan(0)
                .WithMessage("Membership ID must be greater than 0.");
        }
    }
}
