using GymWebApp.Data.Entities;

namespace GymWebApp.Data.Repositories.Interfaces;

public interface IGroupTrainingRepository : IRepository<GroupTraining>
{
    Task<IEnumerable<GroupTraining>> GetAllGroupTrainingsWithTrainersAsync();
}
