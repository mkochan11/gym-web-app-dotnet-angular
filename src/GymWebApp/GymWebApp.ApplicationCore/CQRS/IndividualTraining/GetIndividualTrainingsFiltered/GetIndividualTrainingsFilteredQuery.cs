using GymWebApp.ApplicationCore.Models.Training;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTraining;

public class GetIndividualTrainingsFilteredQuery : IRequest<IEnumerable<IndividualTrainingWebModel>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TrainerIds { get; set; }
    public string? ClientIds { get; set; }
}