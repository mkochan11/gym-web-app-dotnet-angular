using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class ChangeMembershipPlan
{
    public class Command : IRequest<GymMembershipWebModel>
    {
        public int MembershipId { get; set; }
        public int NewPlanId { get; set; }
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
            var membership = await _membershipRepository.GetByIdWithDetailsAsync(command.MembershipId);
            
            if (membership == null)
            {
                throw new NotFoundException("GymMembership", command.MembershipId);
            }

            if (membership.Status != MembershipStatus.Active)
            {
                throw new NotActiveMembershipException(
                    $"Cannot change plan. Membership is not active. Current status: {membership.Status}");
            }

            var newPlan = await _planRepository.GetByIdAsync(command.NewPlanId);
            
            if (newPlan == null)
            {
                throw new NotFoundException("MembershipPlan", command.NewPlanId);
            }

            if (!newPlan.IsActive)
            {
                throw new BusinessRuleViolationException("Cannot change to inactive plan.");
            }

            if (newPlan.Id == membership.MembershipPlanId)
            {
                throw new BusinessRuleViolationException("Please select a different plan.");
            }

            var oldPlan = membership.MembershipPlan!;
            var today = DateTime.UtcNow.Date;
            
            var billingDayOfMonth = membership.StartDate.Day;
            var currentMonth = today.Month;
            var currentYear = today.Year;
            
            DateTime nextBillingDate;
            if (today.Day < billingDayOfMonth)
            {
                nextBillingDate = new DateTime(currentYear, currentMonth, billingDayOfMonth);
            }
            else
            {
                nextBillingDate = new DateTime(currentYear, currentMonth, billingDayOfMonth).AddMonths(1);
            }
            
            var daysRemainingInPeriod = (nextBillingDate - today).Days;
            
            var oldDailyRate = oldPlan.Price / 30;
            var creditAmount = oldDailyRate * daysRemainingInPeriod;

            await _paymentRepository.CancelFuturePaymentsAsync(membership.Id, today, ct);

            var payments = new List<Payment>();
            var startDate = today;
            
            for (int i = 0; i < newPlan.DurationInMonths; i++)
            {
                var dueDate = startDate.AddMonths(i);
                var amount = newPlan.Price;

                if (i == 0)
                {
                    amount = newPlan.Price - creditAmount;
                    
                    if (amount < 0)
                    {
                        payments.Add(new Payment
                        {
                            GymMembershipId = membership.Id,
                            DueDate = dueDate,
                            Amount = 0,
                            Status = PaymentStatus.Paid,
                            PaymentMethod = PaymentMethod.Other,
                            IsSuccessful = true,
                            CreatedById = command.UpdatedById ?? string.Empty
                        });
                        
                        var remainingCredit = Math.Abs(amount);
                        var nextMonthAmount = Math.Max(0, newPlan.Price - remainingCredit);
                        
                        if (nextMonthAmount > 0)
                        {
                            payments.Add(new Payment
                            {
                                GymMembershipId = membership.Id,
                                DueDate = dueDate.AddMonths(1),
                                Amount = nextMonthAmount,
                                Status = PaymentStatus.Pending,
                                PaymentMethod = PaymentMethod.Card,
                                IsSuccessful = true,
                                CreatedById = command.UpdatedById ?? string.Empty
                            });
                        }
                        
                        continue;
                    }
                }

                payments.Add(new Payment
                {
                    GymMembershipId = membership.Id,
                    DueDate = dueDate,
                    Amount = amount,
                    Status = PaymentStatus.Pending,
                    PaymentMethod = PaymentMethod.Card,
                    IsSuccessful = true,
                    CreatedById = command.UpdatedById ?? string.Empty
                });
            }

            if (payments.Count > 0)
            {
                await _paymentRepository.AddRangeAsync(payments, ct);
                await _paymentRepository.SaveChangesAsync();
            }

            membership.MembershipPlanId = command.NewPlanId;
            membership.StartDate = today;
            membership.EndDate = today.AddMonths(newPlan.DurationInMonths);
            membership.UpdatedAt = DateTime.UtcNow;
            membership.UpdatedById = command.UpdatedById ?? string.Empty;

            _membershipRepository.Update(membership);
            await _membershipRepository.SaveChangesAsync();

            membership = await _membershipRepository.GetByIdWithDetailsAsync(membership.Id)
                ?? throw new NotFoundException("Membership", membership.Id);

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

            RuleFor(x => x.NewPlanId)
                .GreaterThan(0)
                .WithMessage("New plan ID must be greater than 0.");
        }
    }
}
