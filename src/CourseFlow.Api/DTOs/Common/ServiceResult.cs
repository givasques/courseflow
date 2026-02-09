using Microsoft.AspNetCore.Identity;

namespace CourseFlow.Api;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public static ServiceResult<T> Unauthorized<T>(string code, string message)
        => new()
        {
            Success = false,
            StatusCode = 401,
            Errors = new() { { code, [message] } }
        };

    public static ServiceResult<T> BadRequest<T>(IEnumerable<IdentityError> errors)
        => new()
        {
            Success = false,
            StatusCode = 400,
            Errors = errors.ToDictionary(e => e.Code, e => new[] { e.Description }) 
        };

    public static ServiceResult<T> BadRequest<T>(string code, string message)
    => new()
    {
        Success = false,
        StatusCode = 400,
        Errors = new() { { code, [message] } }
    };

    public static ServiceResult<T> Ok<T> (T data)
        => new()
        {
            Success = true,
            StatusCode = 200,
            Data = data
        };
}
