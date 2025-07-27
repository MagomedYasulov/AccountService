using AccountService.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace AccountService.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly IOptions<ApiBehaviorOptions> _options;
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IOptions<ApiBehaviorOptions> options)
    {
        _options = options;
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        switch (exception)
        {
            case ServiceException serviceException:
                HandleServiceException(context , serviceException);
                return;
            case ValidationException validationException:
                HandleValidationException(context, validationException);
                return;
            default:
                throw exception;
        }
    }

    private void HandleValidationException(ExceptionContext context, ValidationException validationException)
    {
        var response = new ProblemDetails
        {
            Title = "Validation Exception",
            Detail = "One or more validation errors occurred",
            Status = StatusCodes.Status400BadRequest,
            Type = _options.Value.ClientErrorMapping[StatusCodes.Status400BadRequest].Link,
            Instance = context.HttpContext.Request.Path
        };

        var errorsDesc = validationException.Errors
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(x => x.Key, x => x.Values);

        response.Extensions.Add("errors", errorsDesc);

        context.Result = new JsonResult(response) { StatusCode = StatusCodes.Status400BadRequest };
    }

    private void HandleServiceException(ExceptionContext context, ServiceException serviceException)
    {
        var response = new ProblemDetails
        {
            Title = serviceException.Title,
            Detail = serviceException.Message,
            Status = serviceException.StatusCode,
            Type = _options.Value.ClientErrorMapping[serviceException.StatusCode].Link,
            Instance = context.HttpContext.Request.Path
        };

        _logger.LogWarning("Api method {path} finished with code {statusCode} and error: {error}",
            context.HttpContext.Request.Path, serviceException.StatusCode, response.Detail);

        context.Result = new JsonResult(response) { StatusCode = serviceException.StatusCode };
    }
}