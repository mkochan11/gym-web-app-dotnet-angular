using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GymWebApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserWebModel>> GetAllAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserWebModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            result.Add(new UserWebModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Role = role ?? "Client",
                CreatedAt = user.CreatedAt
            });
        }

        return result;
    }

    public async Task<UserWebModel?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        return new UserWebModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Role = role ?? "Client",
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<(ApplicationUser User, string Error)?> CreateAsync(CreateUserWebModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (user, errors);
        }

        await _userManager.AddToRoleAsync(user, model.Role);

        return null;
    }

    public Task<List<UserRole>> GetAllRolesAsync()
    {
        var roles = Enum.GetValues<UserRole>().ToList();
        return Task.FromResult(roles);
    }

    public async Task AddToRoleAsync(ApplicationUser user, UserRole role)
    {
        var roleName = role.ToString();
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<string?> UpdateAsync(string id, string email, string firstName, string lastName, string? phoneNumber, string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return "User not found";

        user.Email = email;
        user.UserName = email;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.PhoneNumber = phoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return errors;
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        }
        await _userManager.AddToRoleAsync(user, role);

        return null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }
}
