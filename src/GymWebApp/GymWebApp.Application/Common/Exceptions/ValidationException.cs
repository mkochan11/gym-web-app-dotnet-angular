using GymWebApp.Domain.Enums;

namespace GymWebApp.Application.Common.Exceptions;

public class ValidationException : AppException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures have occurred.", ErrorCode.ValidationError)
    {
        Errors = new Dictionary<string, string[]>(errors);
    }
}