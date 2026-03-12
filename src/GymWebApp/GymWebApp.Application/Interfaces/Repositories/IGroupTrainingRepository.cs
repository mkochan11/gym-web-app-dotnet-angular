using GymWebApp.Application.DTOs.GroupTraining;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IGroupTrainingRepository : IRepository<GroupTraining>
{
    Task<IEnumerable<GroupTraining>> GetAllGroupTrainingsWithDetailsAsync();
    Task<IEnumerable<GroupTraining>> GetFilteredGroupTrainingsAsync(GroupTrainingFiltersDto filters);
    Task<bool> ExistsOverlappingAsync(int trainerId, DateTime start, DateTime end, CancellationToken ct);
    Task<bool> ExistsOverlappingExcludingAsync(int trainerId, DateTime start, DateTime end, int excludeId, CancellationToken ct);
}
