namespace GymWebApp.Application.WebModels.Client;

public class ClientDetailsWebModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string? Address { get; set; }
    
    public ClientMembershipWebModel? CurrentMembership { get; set; }
}

public class ClientMembershipWebModel
{
    public int Id { get; set; }
    public string PlanName { get; set; } = null!;
    public string PlanDescription { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public bool CanAccessGroupTraining { get; set; }
    public bool CanAccessPersonalTraining { get; set; }
}
