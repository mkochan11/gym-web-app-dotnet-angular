using GymWebApp.Application.DTOs.GroupTraining;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Training;
using MediatR;

namespace GymWebApp.Application.CQRS.GroupTrainings;

public static class GetGroupTrainingsFiltered
{
    public class Handler : IRequestHandler<GetGroupTrainingsFilteredQuery, IEnumerable<GroupTrainingWebModel>>
    {
        private readonly IGroupTrainingRepository _groupTrainingRepository;

        public Handler(IGroupTrainingRepository groupTrainingRepository)
        {
            _groupTrainingRepository = groupTrainingRepository;
        }

        public async Task<IEnumerable<GroupTrainingWebModel>> Handle(GetGroupTrainingsFilteredQuery request, CancellationToken cancellationToken)
        {
            var filtersDto = new GroupTrainingFiltersDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TrainerIds = request.TrainerIds.ToIntList(),
                TrainingTypeIds = request.TrainingTypeIds.ToIntList()
            };

            var trainings = await _groupTrainingRepository.GetFilteredGroupTrainingsAsync(filtersDto);
            return trainings.Select(gt => gt.ToGroupTrainingWebModel()).ToList();
        }
    }
}

public record GetGroupTrainingsFilteredQuery(
    DateTime? StartDate,
    DateTime? EndDate,
    string? TrainerIds,
    string? TrainingTypeIds
) : IRequest<IEnumerable<GroupTrainingWebModel>>;