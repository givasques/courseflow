namespace CourseFlow.Api.Entities;

public sealed class Enrollment
{
    public required string CourseId { get; set; }
    public Course? Course { get; set; }
    public required string StudentId { get; set; }
    public Student? Student { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public DateTime EnrollmentDateUtc { get; set; } = DateTime.UtcNow;
}

public enum EnrollmentStatus
{
    Active = 1,
    Cancelled
}
