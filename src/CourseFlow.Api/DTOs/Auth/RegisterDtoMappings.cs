using CourseFlow.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Npgsql.Replication;

namespace CourseFlow.Api.DTOs.Auth;

public static class RegisterDtoMappings
{
    public static IdentityUser ToIdentityUser(this RegisterDto registerUserDto)
    {
        return new IdentityUser
        {
            UserName = registerUserDto.Email,
            Email = registerUserDto.Email, 
        };
    }

    public static Student ToStudent(this RegisterDto registerUserDto)
    {
        return new Student
        {
            Id = $"s_{Guid.NewGuid()}",
            Email = registerUserDto.Email,
            FullName = registerUserDto.FullName,
            RegisteredAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    public static Instructor ToInstructor(this RegisterDto registerUserDto)
    {
        return new Instructor
        {
            Id = $"i_{Guid.NewGuid()}",
            Email = registerUserDto.Email,
            FullName = registerUserDto.FullName,
            RegisteredAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }
}
