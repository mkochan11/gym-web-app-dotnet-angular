using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Payment;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.Payments;

public static class GetClientPaymentSchedule
{
    public class Query : IRequest<ClientPaymentScheduleDto>
    {
        public int ClientId { get; set; }
    }

    public class Handler : IRequestHandler<Query, ClientPaymentScheduleDto>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IGymMembershipRepository _gymMembershipRepository;

        public Handler(
            IClientRepository clientRepository,
            IGymMembershipRepository gymMembershipRepository)
        {
            _clientRepository = clientRepository;
            _gymMembershipRepository = gymMembershipRepository;
        }

        public async Task<ClientPaymentScheduleDto> Handle(Query query, CancellationToken ct)
        {
            var client = await _clientRepository.GetByIdAsync(query.ClientId);
            if (client == null)
            {
                throw new NotFoundException("Client", query.ClientId);
            }

            var membership = await _gymMembershipRepository.GetByClientIdWithDetailsAsync(query.ClientId);
            
            var dto = new ClientPaymentScheduleDto
            {
                ClientId = client.Id,
                ClientName = client.Name,
                ClientSurname = client.Surname,
                MembershipId = membership?.Id,
                PlanName = membership?.MembershipPlan?.Type,
                IsActive = membership?.Status == MembershipStatus.Active,
                StartDate = membership?.StartDate,
                EndDate = membership?.EndDate,
                Payments = new List<PaymentDto>()
            };

            if (membership?.Payments != null)
            {
                dto.Payments = membership.Payments
                    .Where(p => !p.Removed)
                    .OrderBy(p => p.DueDate)
                    .Select(p => new PaymentDto
                    {
                        Id = p.Id,
                        MembershipId = p.GymMembershipId,
                        DueDate = p.DueDate,
                        Amount = p.Amount,
                        Status = p.Status.ToString(),
                        PaidDate = p.PaidDate,
                        PaymentMethod = p.PaymentMethod.ToString(),
                        TransactionId = p.TransactionId
                    })
                    .ToList();

                dto.TotalPayments = dto.Payments.Count;
                dto.PaidPayments = dto.Payments.Count(p => p.Status == PaymentStatus.Paid.ToString());
                dto.PendingPayments = dto.Payments.Count(p => p.Status == PaymentStatus.Pending.ToString());
                dto.OverduePayments = dto.Payments.Count(p => p.Status == PaymentStatus.Overdue.ToString());
            }

            return dto;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ClientId)
                .GreaterThan(0)
                .WithMessage("Client ID must be greater than 0.");
        }
    }
}
