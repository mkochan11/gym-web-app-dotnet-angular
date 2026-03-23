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
        public int MembershipId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public string CardholderName { get; set; } = string.Empty;
        public string? CreatedById { get; set; }
    }

    public class Handler : IRequestHandler<Command, PaymentResultWebModel>
    {
        private readonly IGymMembershipRepository _gymMembershipRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IClientRepository _clientRepository;

        public Handler(
            IGymMembershipRepository gymMembershipRepository,
            IPaymentRepository paymentRepository,
            IClientRepository clientRepository)
        {
            _gymMembershipRepository = gymMembershipRepository;
            _paymentRepository = paymentRepository;
            _clientRepository = clientRepository;
        }

        public async Task<PaymentResultWebModel> Handle(Command command, CancellationToken ct)
        {
            var client = await _clientRepository.GetByAccountIdAsync(command.CreatedById!);
            if (client == null)
            {
                throw new NotFoundException("Client", command.CreatedById);
            }

            var membership = await _gymMembershipRepository.GetByIdWithDetailsAsync(command.MembershipId);
            if (membership == null)
            {
                throw new NotFoundException("GymMembership", command.MembershipId);
            }

            if (membership.ClientId != client.Id)
            {
                throw new GymWebApp.Application.Common.Exceptions.ValidationException("Payment does not belong to this client", new Dictionary<string, string[]>());
            }

            var pendingPayment = membership.Payments
                .Where(p => p.Status == PaymentStatus.Pending)
                .OrderBy(p => p.DueDate)
                .FirstOrDefault();

            if (pendingPayment == null)
            {
                return PaymentResultWebModel.FailureResult("No pending payments found for this membership.");
            }

            var paymentSuccessful = SimulatePaymentProcessing(command.CardNumber, command.Cvv);
            if (!paymentSuccessful)
            {
                return PaymentResultWebModel.FailureResult("Payment failed. Please check your card details and try again.");
            }

            pendingPayment.PaidDate = DateTime.UtcNow;
            pendingPayment.Status = PaymentStatus.Paid;
            pendingPayment.PaymentMethod = PaymentMethod.Card;
            pendingPayment.IsSuccessful = true;
            pendingPayment.ProcessedAt = DateTime.UtcNow;
            pendingPayment.ProcessedBy = command.CreatedById;

            _paymentRepository.Update(pendingPayment);
            await _paymentRepository.SaveChangesAsync();

            var allPayments = membership.Payments.Where(p => p.Status != PaymentStatus.Cancelled).ToList();
            var allDuePaid = allPayments.All(p => p.Status == PaymentStatus.Paid || p.DueDate > DateTime.UtcNow);

            if (allDuePaid)
            {
                membership.IsActive = true;
                _gymMembershipRepository.Update(membership);
                await _gymMembershipRepository.SaveChangesAsync();
            }

            return PaymentResultWebModel.SuccessResult(
                membership.Id,
                pendingPayment.Id,
                pendingPayment.Amount,
                PaymentMethod.Card.ToString(),
                membership.StartDate,
                membership.EndDate,
                membership.MembershipPlan?.Type ?? "Unknown");
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
            RuleFor(x => x.MembershipId)
                .GreaterThan(0)
                .WithMessage("Membership ID must be greater than 0.");

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