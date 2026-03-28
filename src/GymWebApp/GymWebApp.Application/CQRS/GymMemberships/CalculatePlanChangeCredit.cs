using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.GymMembership;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.GymMemberships;

public static class CalculatePlanChangeCredit
{
    public class Query : IRequest<CreditCalculationResult>
    {
        public int MembershipId { get; set; }
        public int NewPlanId { get; set; }
    }

    public class Handler : IRequestHandler<Query, CreditCalculationResult>
    {
        private readonly IGymMembershipRepository _gymMembershipRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public Handler(IGymMembershipRepository gymMembershipRepository, IMembershipPlanRepository membershipPlanRepository)
        {
            _gymMembershipRepository = gymMembershipRepository;
            _membershipPlanRepository = membershipPlanRepository;
        }

        public async Task<CreditCalculationResult> Handle(Query query, CancellationToken ct)
        {
            var membership = await _gymMembershipRepository.GetByIdWithDetailsAsync(query.MembershipId);
            
            if (membership == null)
            {
                throw new NotFoundException("GymMembership", query.MembershipId);
            }

            if (membership.Status != MembershipStatus.Active)
            {
                throw new NotActiveMembershipException(
                    $"Cannot calculate credit. Membership is not active. Current status: {membership.Status}");
            }

            var newPlan = await _membershipPlanRepository.GetByIdAsync(query.NewPlanId);
            
            if (newPlan == null)
            {
                throw new NotFoundException("MembershipPlan", query.NewPlanId);
            }

            if (!newPlan.IsActive)
            {
                throw new BusinessRuleViolationException("Cannot change to inactive plan.");
            }

            if (newPlan.Id == membership.MembershipPlanId)
            {
                throw new BusinessRuleViolationException("Please select a different plan.");
            }

            var currentPlan = membership.MembershipPlan!;
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
            
            var oldDailyRate = currentPlan.Price / 30;
            var creditAmount = oldDailyRate * daysRemainingInPeriod;
            var newMonthlyAmount = newPlan.Price;
            var firstPaymentAmount = newPlan.Price - creditAmount;

            decimal totalDifference;
            if (firstPaymentAmount < 0)
            {
                totalDifference = 0;
            }
            else
            {
                totalDifference = creditAmount - newMonthlyAmount;
            }

            return new CreditCalculationResult
            {
                UnusedDays = daysRemainingInPeriod,
                CreditAmount = Math.Round(creditAmount, 2),
                NewMonthlyAmount = newPlan.Price,
                TotalDifference = Math.Round(totalDifference, 2),
                FirstPaymentAmount = Math.Max(0, Math.Round(firstPaymentAmount, 2)),
                CurrentPlanId = currentPlan.Id,
                NewPlanId = newPlan.Id,
                CurrentPlanName = currentPlan.Type,
                NewPlanName = newPlan.Type,
                CurrentPlanPrice = currentPlan.Price,
                NewPlanPrice = newPlan.Price,
                IsUpgrade = newPlan.Price > currentPlan.Price,
                CurrentPlanCanReserveTrainings = currentPlan.CanReserveTrainings,
                NewPlanCanReserveTrainings = newPlan.CanReserveTrainings,
                CurrentPlanCanAccessGroupTraining = currentPlan.CanAccessGroupTraining,
                NewPlanCanAccessGroupTraining = newPlan.CanAccessGroupTraining,
                CurrentPlanCanAccessPersonalTraining = currentPlan.CanAccessPersonalTraining,
                NewPlanCanAccessPersonalTraining = newPlan.CanAccessPersonalTraining,
                CurrentPlanCanReceiveTrainingPlans = currentPlan.CanReceiveTrainingPlans,
                NewPlanCanReceiveTrainingPlans = newPlan.CanReceiveTrainingPlans,
                CurrentPlanMaxTrainingsPerMonth = currentPlan.MaxTrainingsPerMonth,
                NewPlanMaxTrainingsPerMonth = newPlan.MaxTrainingsPerMonth
            };
        }
    }

    public class Validator : AbstractValidator<Query>
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
