using GymWebApp.Application.WebModels.Employee;

namespace GymWebApp.Application.WebModels.Shift;

public class ShiftWebModel
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EmployeeWebModel Employee { get; set; } = null!;
    public string[] Statuses { get; set; } = [];
}