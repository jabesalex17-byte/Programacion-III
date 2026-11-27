namespace GymHub.Api.Core;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await Write(context, StatusCodes.Status404NotFound, new { error = ex.Message });
        }
        catch (DataValidationException ex)
        {
            await Write(context, StatusCodes.Status422UnprocessableEntity, new { errors = ex.Errors });
        }
    }

    private static Task Write(HttpContext context, int status, object body)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(body);
    }
}
