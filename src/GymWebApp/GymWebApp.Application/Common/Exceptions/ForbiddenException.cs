using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Common.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string message)
        : base(message, ErrorCode.Forbidden)
    {
    }
}