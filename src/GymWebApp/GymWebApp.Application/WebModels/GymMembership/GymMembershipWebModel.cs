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
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
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
            StartDate = membership.StartDate,
            EndDate = membership.EndDate,
            IsActive = membership.IsActive,
            IsCancelled = membership.IsCancelled,
            CancelledAt = membership.CancelledAt,
            CancellationReason = membership.CancellationReason
        };
    }
}
