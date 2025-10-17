using GymWebApp.Data;
using GymWebApp.Data.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GymWebApp.WebAPI.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                Log.Information("Starting database initialization...");
                
                var db = services.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();
                
                Log.Information("Database migrations applied successfully");
                
                await CreateRolesAsync(services);
                
                Log.Information("Database initialized successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        private static async Task CreateRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var roleNames = RoleHelper.GetAllRoleNames();

            Log.Information("Creating user roles: {RoleNames}", string.Join(", ", roleNames));

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    Log.Information("Created role: {RoleName}", roleName);
                }
                else
                {
                    Log.Debug("Role already exists: {RoleName}", roleName);
                }
            }
            
            Log.Information("User roles creation completed");
        }
    }
}
