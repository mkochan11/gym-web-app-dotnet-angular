using GymWebApp.ApplicationCore.Extensions;
using GymWebApp.ApplicationCore.Models.Training;
using GymWebApp.Data.DTOs;
using GymWebApp.Data.Repositories.Interfaces;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.GroupTrainings;

public class GetGroupTrainingsFilteredQueryHandler : IRequestHandler<GetGroupTrainingsFilteredQuery, IEnumerable<GroupTrainingWebModel>>
{
    private readonly IGroupTrainingRepository _groupTrainingRepository;

    public GetGroupTrainingsFilteredQueryHandler(
        IGroupTrainingRepository groupTrainingRepository)
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