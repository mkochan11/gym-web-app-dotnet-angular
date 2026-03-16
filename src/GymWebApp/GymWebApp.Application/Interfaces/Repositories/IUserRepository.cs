using GymWebApp.Application.WebModels.User;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<List<UserWebModel>> GetAllAsync();
    Task<UserWebModel?> GetByIdAsync(string id);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<(ApplicationUser User, string Error)?> CreateAsync(CreateUserWebModel model);
    Task<List<UserRole>> GetAllRolesAsync();
    Task AddToRoleAsync(ApplicationUser user, UserRole role);
}
