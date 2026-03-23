namespace GymWebApp.Application.WebModels.Payment;

public class PaymentDto
{
    public int Id { get; set; }
    public int MembershipId { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PaidDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
}