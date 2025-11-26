using GymWebApp.Data.DTOs;
using GymWebApp.Data.Entities;

namespace GymWebApp.Data.Repositories.Interfaces;

public interface IGroupTrainingRepository : IRepository<GroupTraining>
{
    Task<IEnumerable<GroupTraining>> GetAllGroupTrainingsWithTrainersAsync();
    Task<IEnumerable<GroupTraining>> GetFilteredGroupTrainingsAsync(GroupTrainingFiltersDto filters);
}
