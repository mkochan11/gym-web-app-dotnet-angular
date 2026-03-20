using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class GetActiveMembership
{
    public class Query : IRequest<GymMembershipWebModel?>
    {
        public int ClientId { get; set; }
    }

    public class Handler : IRequestHandler<Query, GymMembershipWebModel?>
    {
        private readonly IGymMembershipRepository _gymMembershipRepository;

        public Handler(IGymMembershipRepository gymMembershipRepository)
        {
            _gymMembershipRepository = gymMembershipRepository;
        }

        public async Task<GymMembershipWebModel?> Handle(Query query, CancellationToken ct)
        {
            var membership = await _gymMembershipRepository.GetActiveMembershipByClientIdAsync(query.ClientId);
            
            if (membership == null)
            {
                return null;
            }

            return GymMembershipWebModel.FromEntity(membership);
        }
    }
}
