
using Microsoft.AspNetCore.Identity;

namespace CourseFlow.Api;

public sealed class StudentRegistrationStrategy(
    ApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager) : IUserRegistrationStrategy
{
    public string Role => Roles.Student;

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto, IdentityUser identityUser)
    {
        var roleResult = await userManager.AddToRoleAsync(identityUser, Roles.Student);
        if (!roleResult.Succeeded)
            return roleResult;

        Student student = dto.ToStudent();
        student.IdentityId = identityUser.Id;

        applicationDbContext.Students.Add(student);
 
        var result = await applicationDbContext.SaveChangesAsync();
        if (result == 0) 
            throw new Exception("Student was not persisted.");

        return IdentityResult.Success;
    }
}
