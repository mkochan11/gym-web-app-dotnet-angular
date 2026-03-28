namespace GymWebApp.Application.WebModels.Client;

public class ClientUserWebModel
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string TemporaryPassword { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
