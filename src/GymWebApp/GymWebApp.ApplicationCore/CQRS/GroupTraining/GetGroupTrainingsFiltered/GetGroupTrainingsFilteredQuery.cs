using GymWebApp.ApplicationCore.Models.Training;
using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.GroupTraining;

public class GetGroupTrainingsFilteredQuery : IRequest<IEnumerable<GroupTrainingWebModel>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TrainerIds { get; set; }
    public string? TrainingTypeIds { get; set; }
}