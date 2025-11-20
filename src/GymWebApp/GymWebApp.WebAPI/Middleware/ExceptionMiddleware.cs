using GymWebApp.ApplicationCore.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace GymWebApp.WebAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Success = false,
            Error = new ErrorDetail()
        };

        switch (exception)
        {
            case AppException appException:
                context.Response.StatusCode = appException.StatusCode;
                response.Error.Code = appException.ErrorCode;
                response.Error.Message = appException.Message;

                if (appException is ValidationException validationException)
                {
                    response.Error.Details = validationException.Errors;
                }
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Error.Code = "INTERNAL_ERROR";
                response.Error.Message = "An internal server error has occurred";
                break;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public bool Success { get; set; }
    public ErrorDetail Error { get; set; } = new();
}

public class ErrorDetail
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
}