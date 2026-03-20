using GymWebApp.Application.Interfaces;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Seeding;
using GymWebApp.Domain.Entities;
using GymWebApp.Infrastructure.Authentication;
using GymWebApp.Infrastructure.Repositories;
using GymWebApp.Infrastructure.Seeding;
using GymWebApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymWebApp.Infrastructure.Extensions;

public static class InfrastructureLayerRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IGroupTrainingRepository, GroupTrainingRepository>();
        services.AddScoped<IIndividualTrainingRepository, IndividualTrainingRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ITrainingTypeRepository, TrainingTypeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMembershipPlanRepository, MembershipPlanRepository>();
        services.AddScoped<IGymMembershipRepository, GymMembershipRepository>();

        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<IUserSeeder, UserSeeder>();
        services.AddScoped<IDomainDataSeeder, DomainDataSeeder>();
        services.AddScoped<IRoleSeeder, RoleSeeder>();

        return services;
    }
}