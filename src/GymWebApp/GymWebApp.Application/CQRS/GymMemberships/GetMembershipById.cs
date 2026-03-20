using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class GetMembershipById
{
    public class Query : IRequest<GymMembershipWebModel>
    {
        public int Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, GymMembershipWebModel>
    {
        private readonly IGymMembershipRepository _gymMembershipRepository;

        public Handler(IGymMembershipRepository gymMembershipRepository)
        {
            _gymMembershipRepository = gymMembershipRepository;
        }

        public async Task<GymMembershipWebModel> Handle(Query query, CancellationToken ct)
        {
            var membership = await _gymMembershipRepository.GetByIdWithDetailsAsync(query.Id);
            
            if (membership == null)
            {
                throw new NotFoundException("GymMembership", query.Id);
            }

            return GymMembershipWebModel.FromEntity(membership);
        }
    }
}
