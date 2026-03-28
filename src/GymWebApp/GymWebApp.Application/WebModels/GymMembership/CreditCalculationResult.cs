namespace GymWebApp.Application.WebModels.GymMembership;

public class CreditCalculationResult
{
    public decimal UnusedDays { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal NewMonthlyAmount { get; set; }
    public decimal TotalDifference { get; set; }
    public decimal FirstPaymentAmount { get; set; }
    public int CurrentPlanId { get; set; }
    public int NewPlanId { get; set; }
    public string CurrentPlanName { get; set; } = string.Empty;
    public string NewPlanName { get; set; } = string.Empty;
    public decimal CurrentPlanPrice { get; set; }
    public decimal NewPlanPrice { get; set; }
    public bool IsUpgrade { get; set; }
    public bool CurrentPlanCanReserveTrainings { get; set; }
    public bool NewPlanCanReserveTrainings { get; set; }
    public bool CurrentPlanCanAccessGroupTraining { get; set; }
    public bool NewPlanCanAccessGroupTraining { get; set; }
    public bool CurrentPlanCanAccessPersonalTraining { get; set; }
    public bool NewPlanCanAccessPersonalTraining { get; set; }
    public bool CurrentPlanCanReceiveTrainingPlans { get; set; }
    public bool NewPlanCanReceiveTrainingPlans { get; set; }
    public int? CurrentPlanMaxTrainingsPerMonth { get; set; }
    public int? NewPlanMaxTrainingsPerMonth { get; set; }
}
