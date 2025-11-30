using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories.Interfaces;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTrainings
{
    public class CreateIndividualTrainingCommandHandler : IRequestHandler<CreateIndividualTrainingCommand, int>
    {
        private readonly IIndividualTrainingRepository _individualTrainingRepository;

        public CreateIndividualTrainingCommandHandler(IIndividualTrainingRepository individualTrainingRepository)
        {
            _individualTrainingRepository = individualTrainingRepository;
        }

        public async Task<int> Handle(CreateIndividualTrainingCommand request, CancellationToken cancellationToken)
        {
            var training = new IndividualTraining
            {
                TrainerId = request.TrainerId,
                Date = request.StartDate.ToUniversalTime(),
                Duration = TimeSpan.FromMinutes((request.EndDate - request.StartDate).TotalMinutes),
                Description = request.Description ?? string.Empty,
                Notes = request.Notes,
                CreatedById = request.CreatedById,
                CreatedAt = DateTime.UtcNow
            };

            await _individualTrainingRepository.AddAsync(training);
            await _individualTrainingRepository.SaveChangesAsync();

            return training.Id;
        }
    }
}