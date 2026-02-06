namespace CourseFlow.Api;

public sealed class RegisterStudentDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required UserType UserType { get; set; }
}

public enum UserType
{
    Student = 1,
    Other
}