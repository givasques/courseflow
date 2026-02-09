using Microsoft.AspNetCore.Identity;

namespace CourseFlow.Api;

public interface IUserRegistrationStrategy
{
    string Role { get; }
    Task<IdentityResult> RegisterAsync(RegisterDto dto, IdentityUser identityUser);
}
