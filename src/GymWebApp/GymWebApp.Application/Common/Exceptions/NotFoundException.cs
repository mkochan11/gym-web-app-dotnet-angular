using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.", ErrorCode.NotFound)
    {
    }

    public NotFoundException(string message)
        : base(message, ErrorCode.NotFound)
    {
    }
}