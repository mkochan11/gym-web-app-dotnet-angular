namespace GymWebApp.ApplicationCore.Common.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected AppException(string message, int statusCode, string errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures have occurred.", 400, "VALIDATION_ERROR")
    {
        Errors = errors;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.", 404, "NOT_FOUND")
    {
    }

    public NotFoundException(string message)
        : base(message, 404, "NOT_FOUND")
    {
    }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message)
        : base(message, 401, "UNAUTHORIZED")
    {
    }
}
