using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Payment;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.Payments;

public static class AcceptPayment
{
    public class Command : IRequest<PaymentResultWebModel>
    {
        public int ClientId { get; set; }
        public int PaymentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public string? ProcessedById { get; set; }
    }

    public class Handler : IRequestHandler<Command, PaymentResultWebModel>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IGymMembershipRepository _gymMembershipRepository;
        private readonly IClientRepository _clientRepository;

        public Handler(
            IPaymentRepository paymentRepository,
            IGymMembershipRepository gymMembershipRepository,
            IClientRepository clientRepository)
        {
            _paymentRepository = paymentRepository;
            _gymMembershipRepository = gymMembershipRepository;
            _clientRepository = clientRepository;
        }

        public async Task<PaymentResultWebModel> Handle(Command command, CancellationToken ct)
        {
            var client = await _clientRepository.GetByIdAsync(command.ClientId);
            if (client == null)
            {
                throw new NotFoundException("Client", command.ClientId);
            }

            var membership = await _gymMembershipRepository.GetByClientIdWithDetailsAsync(command.ClientId);
            if (membership == null)
            {
                return PaymentResultWebModel.FailureResult("No active membership found for this client.");
            }

            var payment = membership.Payments.FirstOrDefault(p => p.Id == command.PaymentId && !p.Removed);
            if (payment == null)
            {
                throw new NotFoundException("Payment", command.PaymentId);
            }

            if (payment.Status == PaymentStatus.Paid)
            {
                return PaymentResultWebModel.FailureResult("This installment is already paid.");
            }

            if (payment.Status == PaymentStatus.Cancelled)
            {
                return PaymentResultWebModel.FailureResult("This installment was cancelled.");
            }

            if (payment.Status != PaymentStatus.Pending && payment.Status != PaymentStatus.Overdue)
            {
                return PaymentResultWebModel.FailureResult("Only Pending or Overdue installments can be paid.");
            }

            payment.PaidDate = DateTime.UtcNow;
            payment.Status = PaymentStatus.Paid;
            payment.PaymentMethod = command.PaymentMethod;
            payment.TransactionId = command.TransactionId;
            payment.IsSuccessful = true;
            payment.ProcessedAt = DateTime.UtcNow;
            payment.ProcessedBy = command.ProcessedById;

            _paymentRepository.Update(payment);
            await _paymentRepository.SaveChangesAsync();

            var allPayments = membership.Payments.Where(p => !p.Removed && p.Status != PaymentStatus.Cancelled).ToList();
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
            RuleFor(x => x.ClientId)
                .GreaterThan(0)
                .WithMessage("Client ID must be greater than 0.");

            RuleFor(x => x.PaymentId)
                .GreaterThan(0)
                .WithMessage("Payment ID must be greater than 0.");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithMessage("Invalid payment method.");

            RuleFor(x => x.TransactionId)
                .NotNull()
                .NotEmpty()
                .When(x => x.PaymentMethod != PaymentMethod.Cash)
                .WithMessage("Transaction ID is required for non-cash payments.");

            RuleFor(x => x.TransactionId)
                .MaximumLength(100)
                .When(x => x.TransactionId != null)
                .WithMessage("Transaction ID cannot exceed 100 characters.");
        }
    }
}
