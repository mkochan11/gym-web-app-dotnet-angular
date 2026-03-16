namespace GymWebApp.Application.WebModels.User;

public class CreateUserWebModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = null!;
}
