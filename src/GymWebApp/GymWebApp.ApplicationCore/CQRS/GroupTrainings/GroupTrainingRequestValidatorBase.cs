using FluentValidation;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.ApplicationCore.CQRS.GroupTrainings;

public class GroupTrainingRequestValidatorBase<T> : AbstractValidator<T> where T : class
{
    protected readonly IGroupTrainingRepository _groupTrainingRepository;

    public GroupTrainingRequestValidatorBase(
        IGroupTrainingRepository groupTrainingRepository)
    {
        _groupTrainingRepository = groupTrainingRepository;
    }

    protected async Task<bool> GroupTrainingExists(int id,  CancellationToken cancellationToken)
    {
        var training = await _groupTrainingRepository.GetByIdAsync(id);
        return training != null;
    }

    protected async Task<bool> GroupTrainingNotCancelled(int id, CancellationToken cancellationToken)
    {
        var training = await _groupTrainingRepository.GetByIdAsync(id);
        return training?.IsCancelled != true;
    }

    protected async Task<bool> GroupTrainingNotStarted(int id, CancellationToken cancellationToken)
    {
        var training = await _groupTrainingRepository.GetByIdAsync(id);
        return training?.Date > DateTime.UtcNow;
    }
}