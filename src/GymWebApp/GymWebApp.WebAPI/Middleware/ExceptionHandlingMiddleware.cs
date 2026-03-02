using GymWebApp.Application.Common.Exceptions;

namespace GymWebApp.WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = MapStatusCode(ex);

            if (ex is ValidationException vex)
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    error = ex.ErrorCode,
                    message = ex.Message,
                    details = vex.Errors
                });
            }
            else
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    error = ex.ErrorCode,
                    message = ex.Message
                });
            }
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                error = 500,
                message = ex.Message
            });
        }
    }

    private static int MapStatusCode(AppException ex) =>
        ex switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            BusinessRuleViolationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
}