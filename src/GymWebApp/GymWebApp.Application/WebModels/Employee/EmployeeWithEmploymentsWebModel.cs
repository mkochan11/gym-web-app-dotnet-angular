namespace GymWebApp.Application.WebModels.Employee;

public class EmployeeWithEmploymentsWebModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public IEnumerable<EmploymentWebModel>? Employments { get; set; }
}