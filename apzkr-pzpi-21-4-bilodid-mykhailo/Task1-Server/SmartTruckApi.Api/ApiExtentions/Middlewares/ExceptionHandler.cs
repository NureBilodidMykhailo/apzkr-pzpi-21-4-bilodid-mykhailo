using SmartTruckApi.Application.Exceptions;
using SmartTruckApi.Domain.Common;
using System.Net;

namespace SmartTruckApi.Api.ApiExtentions.Middlewares;

public class ExceptionHandler
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var message = exception.Message;
        var statusCode = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case EntityAlreadyExistsException<EntityBase> entityAlreadyExistsException:
                message = entityAlreadyExistsException.Message;
                statusCode = HttpStatusCode.Conflict;
                break;

            case EntityNotFoundException<EntityBase> entityNotFoundException:
                message = entityNotFoundException.Message;
                statusCode = HttpStatusCode.NotFound;
                break;

            case InvalidEmailException invalidEmailException:
                message = invalidEmailException.Message;
                statusCode = HttpStatusCode.UnprocessableEntity;
                break;

            case InvalidPhoneNumberException invalidPhoneNumberException:
                message = invalidPhoneNumberException.Message;
                statusCode = HttpStatusCode.UnprocessableEntity;
                break;

            case AccessViolationException accessViolationException:
                message = accessViolationException.Message;
                statusCode = HttpStatusCode.Forbidden;
                break;

            default:
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        var response = new
        {
            message = message
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
