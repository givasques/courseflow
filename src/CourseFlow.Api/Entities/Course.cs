namespace CourseFlow.Api.Entities;

public sealed class Course
{
    public string Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public CourseCategory Category { get; set; }
    public required int WorkloadHours { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public string? InstructorId { get; set; }
    public Instructor? Instructor { get; set; }
}
