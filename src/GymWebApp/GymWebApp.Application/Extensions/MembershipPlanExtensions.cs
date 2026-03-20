using GymWebApp.Application.WebModels.MembershipPlan;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Extensions;

public static class MembershipPlanExtensions
{
    public static MembershipPlanWebModel ToMembershipPlanWebModel(this MembershipPlan plan) =>
        new MembershipPlanWebModel
        {
            Id = plan.Id,
            Type = plan.Type,
            Description = plan.Description,
            Price = plan.Price,
            DurationTime = plan.DurationTime,
            DurationInMonths = plan.DurationInMonths,
            CanReserveTrainings = plan.CanReserveTrainings,
            CanAccessGroupTraining = plan.CanAccessGroupTraining,
            CanAccessPersonalTraining = plan.CanAccessPersonalTraining,
            CanReceiveTrainingPlans = plan.CanReceiveTrainingPlans,
            MaxTrainingsPerMonth = plan.MaxTrainingsPerMonth,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
}
