using GymWebApp.Data.DTOs;
using GymWebApp.Data.Entities;

namespace GymWebApp.Data.Repositories.Interfaces;

public interface IIndividualTrainingRepository : IRepository<IndividualTraining>
{
    Task<IEnumerable<IndividualTraining>> GetAllIndividualTrainingsWithTrainersAsync();
    Task<IEnumerable<IndividualTraining>> GetFilteredIndividualTrainingsAsync(IndividualTrainingFiltersDto filters);
}
