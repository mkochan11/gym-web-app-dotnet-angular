namespace GymWebApp.Application.WebModels.Employee;

public class EmploymentWebModel
{
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
        
    public decimal? HourlyRate { get; set; }
}