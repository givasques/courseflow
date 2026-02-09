namespace CourseFlow.Api;

public class Instructor
{
    public string Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Course> Courses { get; set; } = [];
    public string IdentityId { get; set; }
}
