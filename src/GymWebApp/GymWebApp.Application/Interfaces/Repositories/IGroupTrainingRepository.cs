using GymWebApp.Application.DTOs.GroupTraining;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IGroupTrainingRepository : IRepository<GroupTraining>
{
    Task<IEnumerable<GroupTraining>> GetAllGroupTrainingsWithTrainersAsync();
    Task<IEnumerable<GroupTraining>> GetFilteredGroupTrainingsAsync(GroupTrainingFiltersDto filters);
    Task<bool> ExistsOverlappingAsync(int trainerId, DateTime start, DateTime end, CancellationToken ct);
}
