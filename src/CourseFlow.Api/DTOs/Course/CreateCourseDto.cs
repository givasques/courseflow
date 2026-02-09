using CourseFlow.Api.Entities;

namespace CourseFlow.Api.DTOs.Course;

public sealed class CreateCourseDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required CourseCategory Category { get; set; }
    public required int WorkloadHours { get; set; }
}
