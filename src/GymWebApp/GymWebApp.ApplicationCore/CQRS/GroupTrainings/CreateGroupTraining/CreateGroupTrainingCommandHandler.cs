using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories.Interfaces;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.GroupTrainings
{
    public class CreateGroupTrainingCommandHandler : IRequestHandler<CreateGroupTrainingCommand, int>
    {
        private readonly IGroupTrainingRepository _groupTrainingRepository;

        public CreateGroupTrainingCommandHandler(IGroupTrainingRepository groupTrainingRepository)
        {
            _groupTrainingRepository = groupTrainingRepository;
        }

        public async Task<int> Handle(CreateGroupTrainingCommand request, CancellationToken cancellationToken)
        {
            var training = new GroupTraining
            {
                TrainerId = request.TrainerId,
                MaxParticipantNumber = request.MaxParticipantNumber,
                TrainingTypeId = request.TrainingTypeId,
                DifficultyLevel = request.DifficultyLevel,
                Date = request.StartDate.ToUniversalTime(),
                Duration = TimeSpan.FromMinutes((request.EndDate - request.StartDate).TotalMinutes),
                Description = request.Description ?? string.Empty,
                Notes = request.Notes,
                CreatedById = request.CreatedById,
                CreatedAt = DateTime.UtcNow
            };

            await _groupTrainingRepository.AddAsync(training);
            await _groupTrainingRepository.SaveChangesAsync();

            return training.Id;
        }
    }
}