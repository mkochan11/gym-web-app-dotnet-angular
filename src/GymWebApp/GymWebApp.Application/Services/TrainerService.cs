using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using System.Collections;

namespace GymWebApp.Application.Services;

public class TrainerService : ITrainerService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IIndividualTrainingRepository _individualTrainingRepository;
    private readonly IGroupTrainingRepository _groupTrainingRepository;

    public TrainerService(
        IEmployeeRepository employeeRepository,
        IGroupTrainingRepository groupTrainingRepository,
        IIndividualTrainingRepository individualTrainingRepository)
    {
        _employeeRepository = employeeRepository;
        _groupTrainingRepository = groupTrainingRepository;
        _individualTrainingRepository = individualTrainingRepository;
    }

    public async Task<bool> IsAvailableAsync(int trainerId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        var hasIndividualConflict =
           await _individualTrainingRepository.ExistsOverlappingAsync(
               trainerId, startTime, endTime, cancellationToken);

        if (hasIndividualConflict)
            return false;

        var hasGroupConflict =
            await _groupTrainingRepository.ExistsOverlappingAsync(
                trainerId, startTime, endTime, cancellationToken);

        return !hasGroupConflict;
    }

    public async Task<bool> IsAvailableExcludingAsync(int trainerId, DateTime startTime, DateTime endTime, int excludeTrainingId, CancellationToken cancellationToken)
    {
        var hasIndividualConflict =
           await _individualTrainingRepository.ExistsOverlappingExcludingAsync(
               trainerId, startTime, endTime, excludeTrainingId, cancellationToken);

        if (hasIndividualConflict)
            return false;

        var hasGroupConflict =
            await _groupTrainingRepository.ExistsOverlappingExcludingAsync(
                trainerId, startTime, endTime, excludeTrainingId, cancellationToken);

        return !hasGroupConflict;
    }
}