using GymWebApp.ApplicationCore.Extensions;
using GymWebApp.ApplicationCore.Models.Training;
using GymWebApp.Data.DTOs;
using GymWebApp.Data.Repositories.Interfaces;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTrainings;

public class GetIndividualTrainingsFilteredQueryHandler : IRequestHandler<GetIndividualTrainingsFilteredQuery, IEnumerable<IndividualTrainingWebModel>>
{
    private readonly IIndividualTrainingRepository _individualTrainingRepository;

    public GetIndividualTrainingsFilteredQueryHandler(
        IIndividualTrainingRepository individualTrainingRepository)
    {
        _individualTrainingRepository = individualTrainingRepository;
    }

    public async Task<IEnumerable<IndividualTrainingWebModel>> Handle(GetIndividualTrainingsFilteredQuery request, CancellationToken cancellationToken)
    {
        var filtersDto = new IndividualTrainingFiltersDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TrainersIds = request.TrainerIds.ToIntList(),
            ClientsIds = request.ClientIds.ToIntList()
        };

        var trainings = await _individualTrainingRepository.GetFilteredIndividualTrainingsAsync(filtersDto);

        return trainings.Select(it => it.ToIndividualTrainingWebModel()).ToList();
    }
}