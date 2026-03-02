using GymWebApp.Application.Interfaces.Seeding;
using Serilog;

namespace GymWebApp.Infrastructure.Seeding;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly IRoleSeeder _roleSeeder;
    private readonly IUserSeeder _userSeeder;
    private readonly IDomainDataSeeder _domainDataSeeder;
    private readonly ApplicationDbContext _context;

    public DatabaseSeeder(
        IRoleSeeder roleSeeder,
        IUserSeeder userSeeder,
        IDomainDataSeeder domainDataSeeder,
        ApplicationDbContext context)
    {
        _roleSeeder = roleSeeder;
        _userSeeder = userSeeder;
        _domainDataSeeder = domainDataSeeder;
        _context = context;
    }

    public async Task SeedAsync()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Log.Information("Starting database seeding...");

            await _roleSeeder.SeedRolesAsync();
            await _userSeeder.SeedDefaultUsersAsync();
            await _domainDataSeeder.SeedAsync();

            await transaction.CommitAsync();
            Log.Information("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Log.Error(ex, "Error occurred during database seeding.");
            throw;
        }
    }
}