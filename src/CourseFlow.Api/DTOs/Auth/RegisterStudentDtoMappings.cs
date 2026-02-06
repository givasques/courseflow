using Microsoft.AspNetCore.Identity;
using Npgsql.Replication;

namespace CourseFlow.Api;

public static class RegisterStudentDtoMappings
{
    public static IdentityUser ToIdentityUser(this RegisterStudentDto registerUserDto)
    {
        return new IdentityUser
        {
            UserName = registerUserDto.Email,
            Email = registerUserDto.Email, 
        };
    }

    public static Student ToStudent(this RegisterStudentDto registerUserDto)
    {
        return new Student
        {
            Id = $"s_{new Guid()}",
            Email = registerUserDto.Email,
            FullName = registerUserDto.FullName,
            RegisteredAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }
}
