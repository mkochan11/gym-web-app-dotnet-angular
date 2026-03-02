using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Common.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message)
        : base(message, ErrorCode.Unauthorized)
    {
    }
}