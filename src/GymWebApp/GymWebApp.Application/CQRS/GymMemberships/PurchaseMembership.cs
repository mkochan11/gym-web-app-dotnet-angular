using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class PurchaseMembership
{
    public class Command : IRequest<GymMembershipWebModel>
    {
        public int ClientId { get; set; }
        public int MembershipPlanId { get; set; }
        public string? UpdatedById { get; set; }
    }

    public class Handler : IRequestHandler<Command, GymMembershipWebModel>
    {
        private readonly IGymMembershipRepository _membershipRepository;
        private readonly IMembershipPlanRepository _planRepository;
        private readonly IPaymentRepository _paymentRepository;

        public Handler(
            IGymMembershipRepository membershipRepository,
            IMembershipPlanRepository planRepository,
            IPaymentRepository paymentRepository)
        {
            _membershipRepository = membershipRepository;
            _planRepository = planRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<GymMembershipWebModel> Handle(Command command, CancellationToken ct)
        {
            var plan = await _planRepository.GetByIdAsync(command.MembershipPlanId)
                ?? throw new NotFoundException("Membership plan", command.MembershipPlanId);

            var hasActive = await _membershipRepository.HasActiveMembershipAsync(command.ClientId);
            if (hasActive)
            {
                throw new GymWebApp.Application.Common.Exceptions.ValidationException("Client already has an active membership", new Dictionary<string, string[]>());
            }

            var membership = new GymMembership
            {
                ClientId = command.ClientId,
                MembershipPlanId = command.MembershipPlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(plan.DurationInMonths),
                IsActive = false,
                IsCancelled = false,
                CreatedById = command.UpdatedById ?? string.Empty
            };

            await _membershipRepository.AddAsync(membership);
            await _membershipRepository.SaveChangesAsync();

            var payments = new List<Payment>();
            var purchaseDate = DateTime.UtcNow;
            
            for (int i = 0; i < plan.DurationInMonths; i++)
            {
                payments.Add(new Payment
                {
                    GymMembershipId = membership.Id,
                    DueDate = purchaseDate.AddMonths(i),
                    Amount = plan.Price,
                    Status = PaymentStatus.Pending,
                    PaymentMethod = PaymentMethod.Card,
                    IsSuccessful = true,
                    CreatedById = command.UpdatedById ?? string.Empty
                });
            }

            await _paymentRepository.AddRangeAsync(payments, ct);
            await _paymentRepository.SaveChangesAsync();

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
        }
    }
}