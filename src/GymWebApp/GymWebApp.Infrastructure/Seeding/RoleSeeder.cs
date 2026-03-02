using GymWebApp.Application.Interfaces.Seeding;
using GymWebApp.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace GymWebApp.Infrastructure.Seeding;

public class RoleSeeder : IRoleSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleSeeder(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedRolesAsync()
    {
        var roleNames = RoleHelper.GetAllRoleNames();

        foreach (var roleName in roleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                Log.Information("Created role: {RoleName}", roleName);
            }
        }
    }
}
