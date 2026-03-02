namespace GymWebApp.Application.Interfaces.Services;

public interface ITrainerService
{
    Task<bool> IsAvailableAsync(int trainerId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);
}
