namespace GymWebApp.Application.Common.Exceptions;

public class NotActiveMembershipException : AppException
{
    public NotActiveMembershipException(string message)
        : base(message, Domain.Enums.ErrorCode.BadRequest)
    {
    }

    public NotActiveMembershipException(int membershipId)
        : base($"Membership ({membershipId}) is not active or does not exist.", Domain.Enums.ErrorCode.BadRequest)
    {
    }
}
