using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.ApplicationCore.CQRS.GroupTrainings;

public class CancelGroupTrainingCommand : IRequest
{
    public int Id { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
}