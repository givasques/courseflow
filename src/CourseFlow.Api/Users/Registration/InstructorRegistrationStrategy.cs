using CourseFlow.Api.Data;
using CourseFlow.Api.DTOs.Auth;
using CourseFlow.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace CourseFlow.Api.Users.Registration;

public class InstructorRegistrationStrategy(
    ApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager) : IUserRegistrationStrategy
{
    public string Role => Roles.Instructor;

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto, IdentityUser identityUser)
    {
        var roleResult = await userManager.AddToRoleAsync(identityUser, Roles.Instructor);
        if (!roleResult.Succeeded)
            return roleResult;

        Instructor instructor = dto.ToInstructor();
        instructor.IdentityId = identityUser.Id;

        applicationDbContext.Instructors.Add(instructor);
 
        var result = await applicationDbContext.SaveChangesAsync();
        if (result == 0) 
            throw new Exception("Instructor was not persisted.");

        return IdentityResult.Success;
    }
}
