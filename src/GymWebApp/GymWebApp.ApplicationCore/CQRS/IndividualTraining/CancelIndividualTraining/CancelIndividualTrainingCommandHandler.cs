using GymWebApp.Data.Repositories.Interfaces;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTraining
{
    public class CancelIndividualTrainingCommandHandler : IRequestHandler<CancelIndividualTrainingCommand>
    {
        private readonly IIndividualTrainingRepository _individualTrainingRepository;

        public CancelIndividualTrainingCommandHandler(
            IIndividualTrainingRepository individualTrainingRepository)
        {
            _individualTrainingRepository = individualTrainingRepository;
        }

        public async Task Handle(CancelIndividualTrainingCommand request, CancellationToken cancellationToken)
        {
            var individualTraining = await _individualTrainingRepository.GetByIdAsync(request.Id);

            individualTraining!.IsCancelled = true;
            individualTraining!.CancellationReason = request.CancellationReason;

            //TODO: sent notification

            await _individualTrainingRepository.SaveChangesAsync();
        }
    }
}