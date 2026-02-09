using Microsoft.AspNetCore.Mvc;

namespace CourseFlow.Api;

public static class ControllerExtensions
{
    public static IActionResult FromServiceResult<T>(this ControllerBase controller, ServiceResult<T> result)
    {
        if (result.Success) 
            return controller.Ok(result.Data);

        var problem = new ProblemDetails
        {
          Status = result.StatusCode,
          Title = "Request failed"  
        };

        problem.Extensions["errors"] = result.Errors;

        return controller.StatusCode(result.StatusCode, problem);
    }
}
