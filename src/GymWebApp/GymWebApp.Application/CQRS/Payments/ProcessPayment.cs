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
        public int? PaymentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
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

            Payment? payment;

            if (command.PaymentId.HasValue)
            {
                payment = membership.Payments.FirstOrDefault(p => p.Id == command.PaymentId.Value);
                if (payment == null)
                {
                    throw new NotFoundException("Payment", command.PaymentId.Value);
                }
            }
            else
            {
                payment = membership.Payments
                    .Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Overdue)
                    .OrderBy(p => p.DueDate)
                    .FirstOrDefault();
            }

            if (payment == null)
            {
                return PaymentResultWebModel.FailureResult("No pending or overdue payments found for this membership.");
            }

            if (payment.Status == PaymentStatus.Paid)
            {
                return PaymentResultWebModel.FailureResult("This installment is already paid.");
            }

            if (payment.Status == PaymentStatus.Cancelled)
            {
                return PaymentResultWebModel.FailureResult("This installment was cancelled.");
            }

            payment.PaidDate = DateTime.UtcNow;
            payment.Status = PaymentStatus.Paid;
            payment.PaymentMethod = command.PaymentMethod;
            payment.TransactionId = command.TransactionId;
            payment.IsSuccessful = true;
            payment.ProcessedAt = DateTime.UtcNow;
            payment.ProcessedBy = command.CreatedById;

            _paymentRepository.Update(payment);
            await _paymentRepository.SaveChangesAsync();

            var allPayments = membership.Payments.Where(p => p.Status != PaymentStatus.Cancelled).ToList();
            var allDuePaid = allPayments.All(p => p.Status == PaymentStatus.Paid || p.DueDate > DateTime.UtcNow);

            if (allDuePaid)
            {
                membership.Status = MembershipStatus.Active;
                _gymMembershipRepository.Update(membership);
                await _gymMembershipRepository.SaveChangesAsync();
            }

            return PaymentResultWebModel.SuccessResult(
                membership.Id,
                payment.Id,
                payment.Amount,
                command.PaymentMethod.ToString(),
                membership.StartDate,
                membership.EndDate,
                membership.MembershipPlan?.Type ?? "Unknown");
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.MembershipId)
                .GreaterThan(0)
                .WithMessage("Membership ID must be greater than 0.");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithMessage("Invalid payment method.");

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .When(x => x.PaymentMethod != PaymentMethod.Cash)
                .WithMessage("Transaction ID is required for non-cash payments.")
                .MaximumLength(100)
                .When(x => x.TransactionId != null)
                .WithMessage("Transaction ID cannot exceed 100 characters.");

            RuleFor(x => x.PaymentId)
                .GreaterThan(0)
                .When(x => x.PaymentId.HasValue)
                .WithMessage("Payment ID must be greater than 0.");
        }
    }
}