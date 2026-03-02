using GymWebApp.Application.Interfaces.Seeding;
using GymWebApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GymWebApp.WebAPI.Extensions;

public class DatabaseInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IDatabaseSeeder _databaseSeeder;

    public DatabaseInitializer(
        ApplicationDbContext context,
        IDatabaseSeeder databaseSeeder)
    {
        _context = context;
        _databaseSeeder = databaseSeeder;
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            Log.Information("Applying database migrations...");
            await _context.Database.MigrateAsync();
            Log.Information("Database migrations applied successfully.");

            await _databaseSeeder.SeedAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
}
