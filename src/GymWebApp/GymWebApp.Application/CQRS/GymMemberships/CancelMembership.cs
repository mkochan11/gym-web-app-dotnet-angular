using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class CancelMembership
{
    public class Command : IRequest<GymMembershipWebModel>
    {
        public int MembershipId { get; set; }
        public string? CancellationReason { get; set; }
        public string? UpdatedById { get; set; }
    }

    public class Handler : IRequestHandler<Command, GymMembershipWebModel>
    {
        private readonly IGymMembershipRepository _gymMembershipRepository;

        public Handler(IGymMembershipRepository gymMembershipRepository)
        {
            _gymMembershipRepository = gymMembershipRepository;
        }

        public async Task<GymMembershipWebModel> Handle(Command command, CancellationToken ct)
        {
            var membership = await _gymMembershipRepository.GetByIdWithDetailsAsync(command.MembershipId);
            
            if (membership == null)
            {
                throw new NotFoundException("GymMembership", command.MembershipId);
            }

            if (membership.IsCancelled)
            {
                throw new NotActiveMembershipException($"Membership ({command.MembershipId}) is already cancelled.");
            }

            var today = DateTime.UtcNow.Date;
            if (membership.EndDate < today)
            {
                throw new NotActiveMembershipException($"Cannot cancel expired membership ({command.MembershipId}).");
            }

            membership.IsCancelled = true;
            membership.CancelledAt = DateTime.UtcNow;
            membership.CancellationReason = command.CancellationReason;
            membership.UpdatedAt = DateTime.UtcNow;
            membership.UpdatedById = command.UpdatedById;

            _gymMembershipRepository.Update(membership);
            await _gymMembershipRepository.SaveChangesAsync();

            return GymMembershipWebModel.FromEntity(membership);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.MembershipId)
                .GreaterThan(0)
                .WithMessage("Membership ID must be greater than 0.");

            RuleFor(x => x.CancellationReason)
                .MaximumLength(500)
                .WithMessage("Cancellation reason cannot exceed 500 characters.");
        }
    }
}
