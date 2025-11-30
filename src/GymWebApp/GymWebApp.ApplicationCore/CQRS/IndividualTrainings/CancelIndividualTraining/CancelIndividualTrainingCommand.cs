using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.IndividualTrainings;

public class CancelIndividualTrainingCommand : IRequest
{
    public int Id { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}