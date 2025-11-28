using MediatR;

namespace GymWebApp.ApplicationCore.CQRS.Shift
{
    public class CancelShiftCommand : IRequest
    {
        public int Id { get; set; }
        public string CancellationReason { get; set; } = string.Empty;
    }
}