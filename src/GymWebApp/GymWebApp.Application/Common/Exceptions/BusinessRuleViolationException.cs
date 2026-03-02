using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Common.Exceptions;

public class BusinessRuleViolationException : AppException
{
    public BusinessRuleViolationException(string message)
        : base(message, ErrorCode.BadRequest)
    {
    }
}