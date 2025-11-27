using GymWebApp.ApplicationCore.Models.Employee;

namespace GymWebApp.ApplicationCore.Models.Shift;

public class ShiftWebModel
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EmployeeWebModel Employee { get; set; } = null!;
    public string Status { get; set; } = null!;
}