using GymWebApp.Application.DTOs;

namespace GymWebApp.Application.Interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateToken(JwtUserData userData);
}
