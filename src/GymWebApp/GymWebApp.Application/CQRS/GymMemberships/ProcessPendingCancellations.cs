using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class ProcessPendingCancellations
{
    public class Command : IRequest<IEnumerable<GymMembershipWebModel>>
    {
        public DateTime EffectiveEndDate { get; set; }
    }

    public class Handler : IRequestHandler<Command, IEnumerable<GymMembershipWebModel>>
    {
        private readonly IGymMembershipRepository _gymMembershipRepository;

        public Handler(IGymMembershipRepository gymMembershipRepository)
        {
            _gymMembershipRepository = gymMembershipRepository;
        }

        public async Task<IEnumerable<GymMembershipWebModel>> Handle(Command command, CancellationToken ct)
        {
            var membershipsToCancel = await _gymMembershipRepository.GetPendingCancellationsAsync(
                command.EffectiveEndDate, ct);

            var result = new List<GymMembershipWebModel>();

            foreach (var membership in membershipsToCancel)
            {
                membership.Status = MembershipStatus.Cancelled;
                membership.CancelledAt = DateTime.UtcNow;
                membership.UpdatedAt = DateTime.UtcNow;

                _gymMembershipRepository.Update(membership);
                result.Add(GymMembershipWebModel.FromEntity(membership));
            }

            if (result.Any())
            {
                await _gymMembershipRepository.SaveChangesAsync();
            }

            return result;
        }
    }
}
