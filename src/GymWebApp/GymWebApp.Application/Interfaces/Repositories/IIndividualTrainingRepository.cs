using GymWebApp.Application.DTOs;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Interfaces.Repositories;

public interface IIndividualTrainingRepository : IRepository<IndividualTraining>
{
    Task<IEnumerable<IndividualTraining>> GetAllIndividualTrainingsWithDetailsAsync();
    Task<IEnumerable<IndividualTraining>> GetFilteredIndividualTrainingsAsync(IndividualTrainingFiltersDto filters);
    Task<bool> ExistsOverlappingAsync(int trainerId, DateTime start, DateTime end, CancellationToken ct);
    Task<bool> ExistsOverlappingExcludingAsync(int trainerId, DateTime start, DateTime end, int excludeId, CancellationToken ct);
}
