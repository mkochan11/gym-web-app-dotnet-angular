using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class ActivateMembership
{
    public class Command : IRequest<GymMembershipWebModel>
    {
        public int ClientId { get; set; }
        public int MembershipPlanId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string? UpdatedById { get; set; }
    }

    public class Handler : IRequestHandler<Command, GymMembershipWebModel>
    {
        private readonly IGymMembershipRepository _membershipRepository;
        private readonly IMembershipPlanRepository _planRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IClientRepository _clientRepository;

        public Handler(
            IGymMembershipRepository membershipRepository,
            IMembershipPlanRepository planRepository,
            IPaymentRepository paymentRepository,
            IClientRepository clientRepository)
        {
            _membershipRepository = membershipRepository;
            _planRepository = planRepository;
            _paymentRepository = paymentRepository;
            _clientRepository = clientRepository;
        }

        public async Task<GymMembershipWebModel> Handle(Command command, CancellationToken ct)
        {
            // Verify client exists
            var client = await _clientRepository.GetByIdAsync(command.ClientId)
                ?? throw new NotFoundException("Client", command.ClientId);

            // Verify plan exists and is active
            var plan = await _planRepository.GetByIdAsync(command.MembershipPlanId)
                ?? throw new NotFoundException("Membership plan", command.MembershipPlanId);

            // Check if client already has an active membership
            var hasActive = await _membershipRepository.HasActiveMembershipAsync(command.ClientId);
            if (hasActive)
            {
                throw new Application.Common.Exceptions.ValidationException("Client already has an active membership", new Dictionary<string, string[]>());
            }

            // Validate payment amount matches plan price
            if (command.Amount != plan.Price)
            {
                throw new Application.Common.Exceptions.ValidationException("Payment amount does not match plan price", new Dictionary<string, string[]>());
            }

            var activationDate = DateTime.UtcNow;

            // Create membership
            var membership = new GymMembership
            {
                ClientId = command.ClientId,
                MembershipPlanId = command.MembershipPlanId,
                StartDate = activationDate,
                EndDate = activationDate.AddMonths(plan.DurationInMonths),
                Status = MembershipStatus.Active,
                CreatedById = command.UpdatedById ?? string.Empty
            };

            await _membershipRepository.AddAsync(membership);
            await _membershipRepository.SaveChangesAsync();

            // Create payment schedule
            var payments = new List<Payment>();
            
            for (int i = 0; i < plan.DurationInMonths; i++)
            {
                var dueDate = activationDate.AddMonths(i);
                var isFirstPayment = i == 0;
                
                payments.Add(new Payment
                {
                    GymMembershipId = membership.Id,
                    DueDate = dueDate,
                    PaidDate = isFirstPayment ? activationDate : null,
                    Amount = plan.Price,
                    Status = isFirstPayment ? PaymentStatus.Paid : PaymentStatus.Pending,
                    PaymentMethod = command.PaymentMethod,
                    TransactionId = isFirstPayment ? command.TransactionId : null,
                    IsSuccessful = true,
                    ProcessedAt = isFirstPayment ? activationDate : null,
                    ProcessedBy = isFirstPayment ? command.UpdatedById : null,
                    CreatedById = command.UpdatedById ?? string.Empty
                });
            }

            await _paymentRepository.AddRangeAsync(payments, ct);
            await _paymentRepository.SaveChangesAsync();

            // Return membership with details
            membership = await _membershipRepository.GetByIdWithDetailsAsync(membership.Id)
                ?? throw new NotFoundException("Membership", membership.Id);

            return GymMembershipWebModel.FromEntity(membership);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ClientId)
                .GreaterThan(0)
                .WithMessage("Client ID is required");

            RuleFor(x => x.MembershipPlanId)
                .GreaterThan(0)
                .WithMessage("Membership plan is required");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithMessage("Valid payment method is required");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Payment amount must be greater than 0");
        }
    }
}
