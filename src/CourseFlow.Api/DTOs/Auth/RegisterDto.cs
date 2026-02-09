namespace CourseFlow.Api.DTOs.Auth;

public sealed class RegisterDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required UserRole UserRole { get; set; }
}

public enum UserRole
{
    Student = 1,
    Instructor
}