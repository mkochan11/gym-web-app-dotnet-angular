namespace GymWebApp.Application.Interfaces.Services;

public interface ITrainerService
{
    Task<bool> IsAvailableAsync(int trainerId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
    Task<bool> IsAvailableExcludingAsync(int trainerId, DateTime startTime, DateTime endTime, int excludeTrainingId, CancellationToken cancellationToken);
}
