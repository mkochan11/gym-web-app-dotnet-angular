namespace GymWebApp.Data.DTOs;

public class ShiftFiltersDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int>? EmployeeIds { get; set; }
}