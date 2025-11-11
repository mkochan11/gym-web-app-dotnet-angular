using GymWebApp.Data.Entities;

namespace GymWebApp.ApplicationCore.Interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateToken(ApplicationUser user);
}
