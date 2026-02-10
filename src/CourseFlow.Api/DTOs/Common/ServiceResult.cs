using CourseFlow.Api;
using Microsoft.AspNetCore.Identity;

namespace CourseFlow.Api;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public static ServiceResult<T> Unauthorized(string code, string message)
        => new()
        {
            Success = false,
            StatusCode = 401,
            Errors = new() { { code, [message] } }
        };

    public static ServiceResult<T> NotFound()
        => new()
        {
            Success = false,
            StatusCode = 404,
        };

    public static ServiceResult<T> NoContent()
        => new()
        {
            Success = true,
            StatusCode = 204,
        };

    public static ServiceResult<T> Forbidden(string code, string message)
        => new()
        {
            Success = false,
            StatusCode = 403,
            Errors = new() { { code, [message] } }
        };

    public static ServiceResult<T> BadRequest(IEnumerable<IdentityError> errors)
        => new()
        {
            Success = false,
            StatusCode = 400,
            Errors = errors.ToDictionary(e => e.Code, e => new[] { e.Description }) 
        };

    public static ServiceResult<T> BadRequest(string code, string message)
    => new()
    {
        Success = false,
        StatusCode = 400,
        Errors = new() { { code, [message] } }
    };

    public static ServiceResult<T> Ok(T data)
        => new()
        {
            Success = true,
            StatusCode = 200,
            Data = data
        };
}