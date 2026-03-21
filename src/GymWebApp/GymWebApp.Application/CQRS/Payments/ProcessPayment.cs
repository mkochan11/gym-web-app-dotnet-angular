using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Payment;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.Payments;

public static class ProcessPayment
{
    public class Command : IRequest<PaymentResultWebModel>
    {
        public int MembershipPlanId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public string CardholderName { get; set; } = string.Empty;
        public string? CreatedById { get; set; }
    }

    public class Handler : IRequestHandler<Command, PaymentResultWebModel>
    {
        private readonly IGymMembershipRepository _gymMembershipRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IClientRepository _clientRepository;

        public Handler(
            IGymMembershipRepository gymMembershipRepository,
            IMembershipPlanRepository membershipPlanRepository,
            IPaymentRepository paymentRepository,
            IClientRepository clientRepository)
        {
            _gymMembershipRepository = gymMembershipRepository;
            _membershipPlanRepository = membershipPlanRepository;
            _paymentRepository = paymentRepository;
            _clientRepository = clientRepository;
        }

        public async Task<PaymentResultWebModel> Handle(Command command, CancellationToken ct)
        {
            var plan = await _membershipPlanRepository.GetByIdAsync(command.MembershipPlanId);
            if (plan == null)
            {
                throw new NotFoundException("MembershipPlan", command.MembershipPlanId);
            }

            var client = await _clientRepository.GetByAccountIdAsync(command.CreatedById!);
            if (client == null)
            {
                throw new NotFoundException("Client", command.CreatedById);
            }

            var paymentSuccessful = SimulatePaymentProcessing(command.CardNumber, command.Cvv);
            if (!paymentSuccessful)
            {
                return PaymentResultWebModel.FailureResult("Payment failed. Please check your card details and try again.");
            }

            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddMonths(plan.DurationInMonths);

            var membership = new GymMembership
            {
                MembershipPlanId = plan.Id,
                ClientId = client.Id,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                IsCancelled = false,
                CreatedById = command.CreatedById,
                CreatedAt = DateTime.UtcNow
            };

            await _gymMembershipRepository.AddAsync(membership);
            await _gymMembershipRepository.SaveChangesAsync();

            var payment = new Payment
            {
                PaymentDate = DateTime.UtcNow,
                GymMembershipId = membership.Id,
                PaymentMethod = PaymentMethod.Card,
                Amount = plan.Price,
                IsSuccessful = true,
                ProcessedAt = DateTime.UtcNow,
                ProcessedBy = command.CreatedById,
                CreatedAt = DateTime.UtcNow,
                CreatedById = command.CreatedById!
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return PaymentResultWebModel.SuccessResult(
                membership.Id,
                payment.Id,
                plan.Price,
                PaymentMethod.Card.ToString(),
                startDate,
                endDate,
                plan.Type);
        }

        private bool SimulatePaymentProcessing(string cardNumber, string cvv)
        {
            return !string.IsNullOrEmpty(cardNumber) && !string.IsNullOrEmpty(cvv);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.MembershipPlanId)
                .GreaterThan(0)
                .WithMessage("Membership plan ID must be greater than 0.");

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Card number is required.")
                .Matches(@"^\d{16}$").WithMessage("Card number must be 16 digits.");

            RuleFor(x => x.ExpiryDate)
                .NotEmpty().WithMessage("Expiry date is required.")
                .Matches(@"^(0[1-9]|1[0-2])\/\d{2}$").WithMessage("Expiry date must be in MM/YY format.");

            RuleFor(x => x.Cvv)
                .NotEmpty().WithMessage("CVV is required.")
                .Matches(@"^\d{3,4}$").WithMessage("CVV must be 3 or 4 digits.");

            RuleFor(x => x.CardholderName)
                .NotEmpty().WithMessage("Cardholder name is required.")
                .MaximumLength(200).WithMessage("Cardholder name cannot exceed 200 characters.");
        }
    }
}