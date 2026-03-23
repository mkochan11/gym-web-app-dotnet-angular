using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Payment;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.Payments;

public static class GetPaymentsByMembership
{
    public class Query : IRequest<IEnumerable<PaymentDto>>
    {
        public int MembershipId { get; set; }
        public string? UserId { get; set; }
    }

    public class Handler : IRequestHandler<Query, IEnumerable<PaymentDto>>
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

        public async Task<IEnumerable<PaymentDto>> Handle(Query query, CancellationToken ct)
        {
            var membership = await _gymMembershipRepository.GetByIdWithDetailsAsync(query.MembershipId);
            if (membership == null)
            {
                throw new NotFoundException("GymMembership", query.MembershipId);
            }

            if (!string.IsNullOrEmpty(query.UserId))
            {
                var client = await _clientRepository.GetByAccountIdAsync(query.UserId);
                if (client == null || membership.ClientId != client.Id)
                {
                    throw new ForbiddenException("You do not have access to this membership's payments.");
                }
            }

            var payments = await _paymentRepository.GetPaymentsByMembershipIdAsync(query.MembershipId);

            return payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                MembershipId = p.GymMembershipId,
                DueDate = p.DueDate,
                Amount = p.Amount,
                Status = p.Status.ToString(),
                PaidDate = p.PaidDate,
                PaymentMethod = p.PaymentMethod.ToString(),
                TransactionId = p.TransactionId
            });
        }
    }
}