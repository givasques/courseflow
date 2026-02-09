using Microsoft.AspNetCore.Mvc;

namespace CourseFlow.Api.Controllers.Extensions;

public static class ControllerExtensions
{
    public static IActionResult FromServiceResult<T>(this ControllerBase controller, ServiceResult<T> result)
    {
        if (result.Success) 
            return controller.Ok(result.Data);

        var problem = new ProblemDetails
        {
          Status = result.StatusCode,
          Title = GetTitle(result.StatusCode),
          Instance = controller.HttpContext.Request.Path
        };

        problem.Extensions["errors"] = result.Errors;
        problem.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;

        return controller.StatusCode(result.StatusCode, problem);
    }

    private static string GetTitle(int statusCode) =>
    statusCode switch
    {
        400 => "Validation failed",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        _ => "Request failed"
    };
}
