using GymWebApp.Application.Seeding.Models;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Seeding;

public interface IUserSeeder
{
    Task<ApplicationUser> CreateUserAsync(UserToAdd userToAdd);
    Task SeedDefaultUsersAsync();
}