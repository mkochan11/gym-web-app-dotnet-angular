using FluentValidation;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTrainings
{
    public class IndividualTrainingRequestValidatorBase<T> : AbstractValidator<T> where T : class
    {
        protected readonly IIndividualTrainingRepository _individualTrainingRepository;

        public IndividualTrainingRequestValidatorBase(
            IIndividualTrainingRepository individualTrainingRepository)
        {
            _individualTrainingRepository = individualTrainingRepository;
        }

        protected async Task<bool> IndividualTrainingExists(int id, CancellationToken cancellationToken)
        {
            var training = await _individualTrainingRepository.GetByIdAsync(id);
            return training != null;
        }

        protected async Task<bool> IndividualTrainingNotCancelled(int id, CancellationToken cancellationToken)
        {
            var training = await _individualTrainingRepository.GetByIdAsync(id);
            return training?.IsCancelled != true;
        }

        protected async Task<bool> IndividualTrainingNotStarted(int id, CancellationToken cancellationToken)
        {
            var training = await _individualTrainingRepository.GetByIdAsync(id);
            return training?.Date > DateTime.UtcNow;
        }
    }
}