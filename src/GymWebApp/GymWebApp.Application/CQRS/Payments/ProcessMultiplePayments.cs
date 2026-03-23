using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Payment;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.Payments;

public static class ProcessMultiplePayments
{
    public class Command : IRequest<IEnumerable<PaymentResultWebModel>>
    {
        public int MembershipId { get; set; }
        public List<int> PaymentIds { get; set; } = [];
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public string? CreatedById { get; set; }
    }

    public class Handler : IRequestHandler<Command, IEnumerable<PaymentResultWebModel>>
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

        public async Task<IEnumerable<PaymentResultWebModel>> Handle(Command command, CancellationToken ct)
        {
            var results = new List<PaymentResultWebModel>();

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
                throw new GymWebApp.Application.Common.Exceptions.ValidationException("Payments do not belong to this client", new Dictionary<string, string[]>());
            }

            if (command.PaymentIds.Count == 0)
            {
                return [PaymentResultWebModel.FailureResult("No payments selected.")];
            }

            if (command.PaymentIds.Count > 3)
            {
                return [PaymentResultWebModel.FailureResult("You can only pay up to 3 payments at once.")];
            }

            foreach (var paymentId in command.PaymentIds)
            {
                var payment = membership.Payments.FirstOrDefault(p => p.Id == paymentId);
                
                if (payment == null)
                {
                    results.Add(PaymentResultWebModel.FailureResult($"Payment #{paymentId} not found."));
                    continue;
                }

                if (payment.Status == PaymentStatus.Paid)
                {
                    results.Add(PaymentResultWebModel.FailureResult($"Payment #{paymentId} is already paid."));
                    continue;
                }

                if (payment.Status == PaymentStatus.Cancelled)
                {
                    results.Add(PaymentResultWebModel.FailureResult($"Payment #{paymentId} was cancelled."));
                    continue;
                }

                payment.PaidDate = DateTime.UtcNow;
                payment.Status = PaymentStatus.Paid;
                payment.PaymentMethod = command.PaymentMethod;
                payment.TransactionId = command.TransactionId;
                payment.IsSuccessful = true;
                payment.ProcessedAt = DateTime.UtcNow;
                payment.ProcessedBy = command.CreatedById;

                _paymentRepository.Update(payment);

                results.Add(PaymentResultWebModel.SuccessResult(
                    membership.Id,
                    payment.Id,
                    payment.Amount,
                    command.PaymentMethod.ToString(),
                    membership.StartDate,
                    membership.EndDate,
                    membership.MembershipPlan?.Type ?? "Unknown"));
            }

            await _paymentRepository.SaveChangesAsync();

            var allPayments = membership.Payments.Where(p => p.Status != PaymentStatus.Cancelled).ToList();
            var allDuePaid = allPayments.All(p => p.Status == PaymentStatus.Paid || p.DueDate > DateTime.UtcNow);

            if (allDuePaid)
            {
                membership.IsActive = true;
                _gymMembershipRepository.Update(membership);
                await _gymMembershipRepository.SaveChangesAsync();
            }

            return results;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.MembershipId)
                .GreaterThan(0)
                .WithMessage("Membership ID must be greater than 0.");

            RuleFor(x => x.PaymentIds)
                .NotEmpty()
                .WithMessage("At least one payment must be selected.");

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
        }
    }
}