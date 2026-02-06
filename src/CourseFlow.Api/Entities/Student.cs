namespace CourseFlow.Api;

public sealed class Student
{
    public string Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public IEnumerable<Enrollment> Enrollments { get; set; } = [];
}
