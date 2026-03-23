using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class RevertCancellation
{
    public class Command : IRequest<GymMembershipWebModel>
    {
        public int MembershipId { get; set; }
        public string? UpdatedById { get; set; }
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

            if (membership.Status != MembershipStatus.PendingCancellation)
            {
                throw new NotActiveMembershipException($"Membership ({command.MembershipId}) is not in pending cancellation status. Current status: {membership.Status}");
            }

            membership.Status = MembershipStatus.Active;
            membership.CancellationRequestedDate = null;
            membership.EffectiveEndDate = null;
            membership.UpdatedAt = DateTime.UtcNow;
            membership.UpdatedById = command.UpdatedById;

            var today = DateTime.UtcNow.Date;
            var cancelledPayments = membership.Payments
                .Where(p => p.Status == PaymentStatus.Cancelled && p.DueDate > today)
                .ToList();

            foreach (var payment in cancelledPayments)
            {
                payment.Status = PaymentStatus.Pending;
                _paymentRepository.Update(payment);
            }

            _gymMembershipRepository.Update(membership);
            await _paymentRepository.SaveChangesAsync();
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
        }
    }
}
