namespace GymWebApp.Application.WebModels.Payment;

public class ClientPaymentScheduleDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientSurname { get; set; } = string.Empty;
    public int? MembershipId { get; set; }
    public string? PlanName { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<PaymentDto> Payments { get; set; } = new();
    public int TotalPayments { get; set; }
    public int PaidPayments { get; set; }
    public int PendingPayments { get; set; }
    public int OverduePayments { get; set; }
}
