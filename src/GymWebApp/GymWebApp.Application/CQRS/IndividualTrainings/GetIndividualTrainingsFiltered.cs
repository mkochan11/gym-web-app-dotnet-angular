using GymWebApp.Application.DTOs;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Training;
using MediatR;

namespace GymWebApp.Application.CQRS.IndividualTrainings;

public static class GetIndividualTrainingsFiltered
{
    public class Handler : IRequestHandler<GetIndividualTrainingsFilteredQuery, IEnumerable<CalendarIndividualTrainingWebModel>>
    {
        private readonly IIndividualTrainingRepository _individualTrainingRepository;
        public Handler(IIndividualTrainingRepository individualTrainingRepository)
        {
            _individualTrainingRepository = individualTrainingRepository;
        }
        public async Task<IEnumerable<CalendarIndividualTrainingWebModel>> Handle(GetIndividualTrainingsFilteredQuery request, CancellationToken cancellationToken)
        {
            var filtersDto = new IndividualTrainingFiltersDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TrainersIds = request.TrainerIds.ToIntList(),
                ClientsIds = request.ClientIds.ToIntList()
            };
            
            var trainings = await _individualTrainingRepository.GetFilteredIndividualTrainingsAsync(filtersDto);
            return trainings.Select(it => it.ToCalendarIndividualTrainingWebModel()).ToList();
        }
    }   
}

public record GetIndividualTrainingsFilteredQuery(
    DateTime? StartDate,
    DateTime? EndDate,
    string? TrainerIds,
    string? ClientIds
) : IRequest<IEnumerable<CalendarIndividualTrainingWebModel>>;