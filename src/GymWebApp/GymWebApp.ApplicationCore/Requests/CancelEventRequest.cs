namespace GymWebApp.ApplicationCore.Requests;

public class CancelEventRequest
{
    public string CancellationReason { get; set; } = string.Empty!;
}