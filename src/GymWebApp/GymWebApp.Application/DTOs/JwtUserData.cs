namespace GymWebApp.Application.DTOs;

public class JwtUserData
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = [];
}