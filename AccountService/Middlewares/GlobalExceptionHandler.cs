using System.Diagnostics;
using AccountService.Domain.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AccountService.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Server error");

        var options = httpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
        var serializeSettings = httpContext.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value.SerializerSettings;

        var problemDetails = new ProblemDetails
        {
            Type = options.Value.ClientErrorMapping[500].Link,
            Title = "Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? httpContext.TraceIdentifier);

        var response = JsonConvert.SerializeObject(new MbError(problemDetails), serializeSettings);

        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsync(response, cancellationToken);
        return true;
    }
}