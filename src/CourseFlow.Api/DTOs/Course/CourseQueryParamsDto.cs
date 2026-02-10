using CourseFlow.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CourseFlow.Api.DTOs.Course;

public sealed class CourseQueryParamsDto
{
    public CourseCategory? Category { get; init; }
    public int Page { get; set; } = 1;
    public int PageSize { get; init; } = 10;
    [FromQuery(Name = "q")]
    public string? Search { get; init; }
}
