using GymWebApp.Data;
using GymWebApp.Data.Entities;
using GymWebApp.Data.Enums;
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
                await CreateUsersAsync(services);

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
        }

        private static async Task CreateUsersAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            if (await userManager.Users.AnyAsync())
            {
                Log.Information("Users already exist in the database. Skipping user creation.");
                return;
            }

            var employeesToAdd = GetDefaultEmployees();
            var clientsToAdd = GetDefaultClients();

            foreach (var userToAdd in employeesToAdd)
                await CreateUserWithEntityAsync(userManager, dbContext, userToAdd, isEmployee: true);

            foreach (var userToAdd in clientsToAdd)
                await CreateUserWithEntityAsync(userManager, dbContext, userToAdd, isEmployee: false);

            await dbContext.SaveChangesAsync();

            await CreateAdminAsync(userManager);
        }


        private static async Task CreateUserWithEntityAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            UserToAdd userToAdd,
            bool isEmployee)
        {
            try
            {
                var existingUser = await userManager.FindByEmailAsync(userToAdd.Email);
                if (existingUser != null)
                {
                    Log.Information("User {Email} already exists, skipping.", userToAdd.Email);
                    return;
                }

                var appUser = new ApplicationUser
                {
                    Email = userToAdd.Email,
                    UserName = userToAdd.Email
                };

                var result = await userManager.CreateAsync(appUser, userToAdd.Password);
                if (!result.Succeeded)
                {
                    Log.Warning("Failed to create user {Email}: {Errors}",
                        userToAdd.Email,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return;
                }

                await userManager.AddToRoleAsync(appUser, userToAdd.Role.ToString());
                var createdUser = await userManager.FindByEmailAsync(userToAdd.Email);

                if (createdUser == null)
                {
                    Log.Warning("Failed to retrieve created user {Email}", userToAdd.Email);
                    return;
                }

                if (isEmployee)
                {
                    var employee = new Employee(
                        createdUser.Id,
                        userToAdd.Name,
                        userToAdd.Surname,
                        userToAdd.Address,
                        userToAdd.DateOfBirth,
                        userToAdd.Gender,
                        userToAdd.EmployeeRole
                    );

                    dbContext.Employees.Add(employee);
                    Log.Information("Added employee {Email} to database.", userToAdd.Email);
                }
                else
                {
                    var client = new Client(
                        createdUser.Id,
                        userToAdd.Name,
                        userToAdd.Surname,
                        userToAdd.Address,
                        userToAdd.DateOfBirth,
                        userToAdd.Gender
                    );

                    dbContext.Clients.Add(client);
                    Log.Information("Added client {Email} to database.", userToAdd.Email);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while creating user {Email}", userToAdd.Email);
            }
        }

        private static async Task CreateAdminAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@gymWebApp.com";
            const string adminPassword = "Admin123!";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null)
                return;

            var adminUser = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
                throw new Exception($"Failed to create admin {adminEmail}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(adminUser, UserRole.Admin.ToString());
        }

        private static List<UserToAdd> GetDefaultEmployees() => new()
        {
            new() { Email = "owner@gymWebApp.com", Password = "Owner123!", EmployeeRole = EmployeeRole.Owner, Role = UserRole.Owner, Name = "Adam", Surname = "Kowalski", Gender = Gender.Male, DateOfBirth = new(1980, 1, 1) },
            new() { Email = "manager@gymWebApp.com", Password = "Manager123!", EmployeeRole = EmployeeRole.Manager, Role = UserRole.Manager, Name = "Jan", Surname = "Menad¿erski", Gender = Gender.Male, DateOfBirth = new(1990, 5, 15) },
            new() { Email = "receptionist@gymWebApp.com", Password = "Receptionist123!", EmployeeRole = EmployeeRole.Receptionist, Role = UserRole.Receptionist, Name = "Anna", Surname = "Recepjowa", Gender = Gender.Female, DateOfBirth = new(2000, 9, 9) },
            new() { Email = "trainer1@gymWebApp.com", Password = "Trainer123!1", EmployeeRole = EmployeeRole.Trainer, Role = UserRole.Trainer, Name = "Krzysztof", Surname = "Trenerski", Gender = Gender.Male, DateOfBirth = new(1995, 2, 2) },
            new() { Email = "trainer2@gymWebApp.com", Password = "Trainer123!2", EmployeeRole = EmployeeRole.Trainer, Role = UserRole.Trainer, Name = "Ola", Surname = "Trenerska", Gender = Gender.Female, DateOfBirth = new(1999, 4, 14) }
        };

        private static List<UserToAdd> GetDefaultClients() => new()
        {
            new() { Email = "client1@gymWebApp.com", Password = "Client123!1", Role = UserRole.Client, Name = "Jakub", Surname = "Kliencki", Gender = Gender.Male, DateOfBirth = new(2001, 1, 15) },
            new() { Email = "client2@gymWebApp.com", Password = "Client123!2", Role = UserRole.Client, Name = "Agnieszka", Surname = "Kliencka", Gender = Gender.Female, DateOfBirth = new(2004, 12, 1) }
        };

        private class UserToAdd
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public UserRole Role { get; set; }
            public EmployeeRole EmployeeRole { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public Gender Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
        }
    }
}
