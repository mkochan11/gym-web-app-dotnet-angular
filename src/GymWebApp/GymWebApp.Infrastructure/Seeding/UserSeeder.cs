using GymWebApp.Application.Interfaces.Seeding;
using GymWebApp.Application.Seeding.Models;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using GymWebApp.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace GymWebApp.Infrastructure.Seeding;

public class UserSeeder : IUserSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserSeeder(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ApplicationUser> CreateUserAsync(UserToAdd userToAdd)
    {
        var existingUser = await _userManager.FindByEmailAsync(userToAdd.Email);
        if (existingUser != null)
        {
            Log.Information("User {Email} already exists.", userToAdd.Email);
            return existingUser;
        }

        var appUser = new ApplicationUser
        {
            Email = userToAdd.Email,
            UserName = userToAdd.Email,
            FirstName = userToAdd.Name,
            LastName = userToAdd.Surname,
            CreatedAt =  DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(appUser, userToAdd.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Log.Warning("Failed to create user {Email}: {Errors}", userToAdd.Email, errors);
            return null!;
        }

        if (userToAdd.Role is not UserRole.Admin)
        {
            await _userManager.AddToRoleAsync(appUser, userToAdd.Role.ToString());
        }
        else
        {
            await _userManager.AddToRolesAsync(appUser, RoleHelper.GetAllRoleNames());
        }
        
        Log.Information("Created user {Email} with role {Role}", userToAdd.Email, userToAdd.Role);

        if (userToAdd.Role != UserRole.Client && userToAdd.EmployeeRole is not null)
        {
            var employee = new Employee(
                appUser.Id,
                userToAdd.Name,
                userToAdd.Surname,
                userToAdd.Address,
                userToAdd.DateOfBirth,
                userToAdd.Gender,
                userToAdd.EmployeeRole.Value
            );
            _context.Employees.Add(employee);
        }
        else if (userToAdd.Role == UserRole.Client)
        {
            var client = new Client(
                appUser.Id,
                userToAdd.Name,
                userToAdd.Surname,
                userToAdd.Address,
                userToAdd.DateOfBirth,
                userToAdd.Gender
            );
            _context.Clients.Add(client);
        }

        return appUser;
    }

    public async Task SeedDefaultUsersAsync()
    {
        var defaultUsers = GetDefaultUsers();

        foreach (var user in defaultUsers)
        {
            await CreateUserAsync(user);
        }

        await _context.SaveChangesAsync();
    }

    private static List<UserToAdd> GetDefaultUsers() => new()
    {
        new() { Email = "admin@gymWebApp.com", Password = "Admin123!", Role = UserRole.Admin, Name = "Admin", Surname = "Admin", Gender = Gender.Male, DateOfBirth = new(2000,1,1) },
        new() { Email = "owner@gymWebApp.com", Password = "Owner123!", EmployeeRole = EmployeeRole.Owner, Role = UserRole.Owner, Name = "Adam", Surname = "Kowalski", Gender = Gender.Male, DateOfBirth = new(1980,1,1) },
        new() { Email = "manager@gymWebApp.com", Password = "Manager123!", EmployeeRole = EmployeeRole.Manager, Role = UserRole.Manager, Name = "Jan", Surname = "Menadżerski", Gender = Gender.Male, DateOfBirth = new(1990,5,15) },
        new() { Email = "receptionist@gymWebApp.com", Password = "Receptionist123!", EmployeeRole = EmployeeRole.Receptionist, Role = UserRole.Receptionist, Name = "Anna", Surname = "Recepjowa", Gender = Gender.Female, DateOfBirth = new(2000,9,9) },
        new() { Email = "trainer1@gymWebApp.com", Password = "Trainer123!1", EmployeeRole = EmployeeRole.Trainer, Role = UserRole.Trainer, Name = "Krzysztof", Surname = "Trenerski", Gender = Gender.Male, DateOfBirth = new(1995,2,2) },
        new() { Email = "trainer2@gymWebApp.com", Password = "Trainer123!2", EmployeeRole = EmployeeRole.Trainer, Role = UserRole.Trainer, Name = "Ola", Surname = "Trenerska", Gender = Gender.Female, DateOfBirth = new(1999,4,14) },
        new() { Email = "client1@gymWebApp.com", Password = "Client123!1", Role = UserRole.Client, Name = "Jakub", Surname = "Kliencki", Gender = Gender.Male, DateOfBirth = new(2001,1,15) },
        new() { Email = "client2@gymWebApp.com", Password = "Client123!2", Role = UserRole.Client, Name = "Agnieszka", Surname = "Kliencka", Gender = Gender.Female, DateOfBirth = new(2004,12,1) },
    };
}
