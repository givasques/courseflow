using CourseFlow.Api.Entities;

namespace CourseFlow.Api.DTOs.Course;

public sealed class CourseDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public CourseCategory Category { get; set; }
    public int WorkloadHours { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public string InstructorId { get; set; }
}
