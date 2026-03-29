namespace GymWebApp.Application.WebModels.Employee;

public class EmploymentWebModel
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}