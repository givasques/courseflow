using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace CourseFlow.Api.Services.ExceptionHandler;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            UnauthorizedAccessException => CreateProblem(
                status: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: exception.Message),

            _ => CreateProblem(
                status: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                detail: "An unexpected error occured.")
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;
        problem.Instance = httpContext.Request.Path;

        httpContext.Response.StatusCode = problem.Status ?? 500;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static ProblemDetails CreateProblem(int status, string title, string detail)
    {
        return new ProblemDetails()
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{status}"
        };
    }
}
