using GymWebApp.Data.Entities;

namespace GymWebApp.ApplicationCore.Services.Interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateToken(ApplicationUser user);
}
