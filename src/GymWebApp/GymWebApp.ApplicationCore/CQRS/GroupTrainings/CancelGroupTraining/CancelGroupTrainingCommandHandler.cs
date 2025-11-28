using GymWebApp.Data.Repositories.Interfaces;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.GroupTrainings
{
    public class CancelGroupTrainingCommandHandler : IRequestHandler<CancelGroupTrainingCommand>
    {
        private readonly IGroupTrainingRepository _groupTrainingRepository;

        public CancelGroupTrainingCommandHandler(
            IGroupTrainingRepository groupTrainingRepository)
        {
            _groupTrainingRepository = groupTrainingRepository;
        }

        public async Task Handle(CancelGroupTrainingCommand request, CancellationToken cancellationToken)
        {
            var groupTraining = await _groupTrainingRepository.GetByIdAsync(request.Id);

            groupTraining!.IsCancelled = true;
            groupTraining!.CancellationReason = request.CancellationReason;

            //TODO: sent notification

            await _groupTrainingRepository.SaveChangesAsync();
        }
    }
}