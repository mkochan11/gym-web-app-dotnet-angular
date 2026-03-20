using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using MediatR;

namespace GymWebApp.Application.CQRS.MembershipPlans;

public static class DeleteMembershipPlan
{
    public class Handler : IRequestHandler<DeleteMembershipPlanCommand, Unit>
    {
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public Handler(IMembershipPlanRepository membershipPlanRepository)
        {
            _membershipPlanRepository = membershipPlanRepository;
        }

        public async Task<Unit> Handle(DeleteMembershipPlanCommand command, CancellationToken ct)
        {
            var plan = await _membershipPlanRepository.GetByIdAsync(command.Id);
            if (plan == null)
            {
                throw new NotFoundException("MembershipPlan", command.Id);
            }

            _membershipPlanRepository.Remove(plan);
            await _membershipPlanRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

public class DeleteMembershipPlanCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
