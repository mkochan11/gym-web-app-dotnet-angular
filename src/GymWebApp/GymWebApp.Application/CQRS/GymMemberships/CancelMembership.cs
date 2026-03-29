using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using GymWebApp.Domain.Enums;
using GymWebApp.Domain.Entities;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class CancelMembership
{
    public class Command : IRequest<GymMembershipWebModel>
    {
        public int MembershipId { get; set; }
        public string? CancellationReason { get; set; }
        public string? UpdatedById { get; set; }
        public bool RequireCancellationReason { get; set; }
    }

    public class Handler : IRequestHandler<Command, GymMembershipWebModel>
    {
        private readonly IGymMembershipRepository _gymMembershipRepository;
        private readonly IPaymentRepository _paymentRepository;

        public Handler(IGymMembershipRepository gymMembershipRepository, IPaymentRepository paymentRepository)
        {
            _gymMembershipRepository = gymMembershipRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<GymMembershipWebModel> Handle(Command command, CancellationToken ct)
        {
            var membership = await _gymMembershipRepository.GetByIdWithDetailsAsync(command.MembershipId);
            
            if (membership == null)
            {
                throw new NotFoundException("GymMembership", command.MembershipId);
            }

            if (membership.Status != MembershipStatus.Active)
            {
                throw new NotActiveMembershipException($"Membership ({command.MembershipId}) is not active. Current status: {membership.Status}");
            }

            var today = DateTime.UtcNow.Date;
            if (membership.EndDate < today)
            {
                throw new NotActiveMembershipException($"Cannot cancel expired membership ({command.MembershipId}).");
            }
            
            var firstDuePayment = membership.Payments
                .Where(p => p.Status == PaymentStatus.Pending)
                .OrderBy(p => p.DueDate)
                .FirstOrDefault();

            var effectiveEndDate = firstDuePayment?.DueDate ?? membership.EndDate;

            membership.Status = MembershipStatus.PendingCancellation;
            membership.CancellationRequestedDate = DateTime.UtcNow;
            membership.EffectiveEndDate = effectiveEndDate;
            membership.CancellationReason = command.CancellationReason;
            membership.UpdatedAt = DateTime.UtcNow;
            membership.UpdatedById = command.UpdatedById;

            await _paymentRepository.CancelFuturePaymentsAsync(membership.Id, today, ct);

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
                .NotEmpty()
                .When(x => x.RequireCancellationReason)
                .WithMessage("Cancellation reason is required for staff-initiated cancellations.");

            RuleFor(x => x.CancellationReason)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.CancellationReason))
                .WithMessage("Cancellation reason cannot exceed 500 characters.");
        }
    }
}
