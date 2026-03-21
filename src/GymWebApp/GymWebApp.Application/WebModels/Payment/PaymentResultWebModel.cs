namespace GymWebApp.Application.WebModels.Payment;

public class PaymentResultWebModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? MembershipId { get; set; }
    public int? PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? PlanName { get; set; }

    public static PaymentResultWebModel SuccessResult(
        int membershipId, 
        int paymentId, 
        decimal amount, 
        string paymentMethod,
        DateTime startDate,
        DateTime endDate,
        string planName)
    {
        return new PaymentResultWebModel
        {
            Success = true,
            Message = $"Payment processed successfully! You now have {planName} membership.",
            MembershipId = membershipId,
            PaymentId = paymentId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            StartDate = startDate,
            EndDate = endDate,
            PlanName = planName
        };
    }

    public static PaymentResultWebModel FailureResult(string message)
    {
        return new PaymentResultWebModel
        {
            Success = false,
            Message = message
        };
    }
}