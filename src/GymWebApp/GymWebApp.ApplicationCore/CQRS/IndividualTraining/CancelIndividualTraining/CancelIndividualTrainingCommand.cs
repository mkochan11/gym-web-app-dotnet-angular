using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTraining;

public class CancelIndividualTrainingCommand : IRequest
{
    public int Id { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}