namespace GymWebApp.Application.WebModels.GymMembership;

public class GymMembershipWebModel
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int MembershipPlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string PlanDescription { get; set; } = string.Empty;
    public decimal PlanPrice { get; set; }
    public int PlanDurationInMonths { get; set; }
    public bool CanReserveTrainings { get; set; }
    public bool CanAccessGroupTraining { get; set; }
    public bool CanAccessPersonalTraining { get; set; }
    public bool CanReceiveTrainingPlans { get; set; }
    public int? MaxTrainingsPerMonth { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Domain.Enums.MembershipStatus Status { get; set; }
    public bool IsActive { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CancellationRequestedDate { get; set; }
    public DateTime? EffectiveEndDate { get; set; }
    public string? CancellationReason { get; set; }

    public static GymMembershipWebModel FromEntity(Domain.Entities.GymMembership membership)
    {
        return new GymMembershipWebModel
        {
            Id = membership.Id,
            ClientId = membership.ClientId,
            ClientName = $"{membership.Client?.Name} {membership.Client?.Surname}".Trim(),
            MembershipPlanId = membership.MembershipPlanId,
            PlanName = membership.MembershipPlan?.Type ?? string.Empty,
            PlanDescription = membership.MembershipPlan?.Description ?? string.Empty,
            PlanPrice = membership.MembershipPlan?.Price ?? 0,
            PlanDurationInMonths = membership.MembershipPlan?.DurationInMonths ?? 0,
            CanReserveTrainings = membership.MembershipPlan?.CanReserveTrainings ?? false,
            CanAccessGroupTraining = membership.MembershipPlan?.CanAccessGroupTraining ?? false,
            CanAccessPersonalTraining = membership.MembershipPlan?.CanAccessPersonalTraining ?? false,
            CanReceiveTrainingPlans = membership.MembershipPlan?.CanReceiveTrainingPlans ?? false,
            MaxTrainingsPerMonth = membership.MembershipPlan?.MaxTrainingsPerMonth,
            StartDate = membership.StartDate,
            EndDate = membership.EndDate,
            Status = membership.Status,
            IsActive = membership.IsActive,
            IsCancelled = membership.IsCancelled,
            CancelledAt = membership.CancelledAt,
            CancellationRequestedDate = membership.CancellationRequestedDate?.ToLocalTime(),
            EffectiveEndDate = membership.EffectiveEndDate,
            CancellationReason = membership.CancellationReason
        };
    }
}
