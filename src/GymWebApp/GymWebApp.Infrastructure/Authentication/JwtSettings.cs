namespace GymWebApp.Infrastructure.Authentication;

public class JwtSettings
{
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public int ExpiryHours { get; set; } = 12;
}