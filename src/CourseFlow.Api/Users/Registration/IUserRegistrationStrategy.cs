using CourseFlow.Api.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace CourseFlow.Api.Users.Registration;

public interface IUserRegistrationStrategy
{
    string Role { get; }
    Task<IdentityResult> RegisterAsync(RegisterDto dto, IdentityUser identityUser);
}
