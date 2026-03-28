namespace GymWebApp.Application.WebModels.Client;

public class ClientListWebModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? MembershipStatus { get; set; }
    public string? CurrentPlanName { get; set; }
}
