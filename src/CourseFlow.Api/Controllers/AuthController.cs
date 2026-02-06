using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CourseFlow.Api;

[ApiController]
[Route("auth")]
public class AuthController(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext identityDbContext, 
    UserManager<IdentityUser> userManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterStudent(RegisterStudentDto registerStudentDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        IdentityUser identityUser = registerStudentDto.ToIdentityUser();

        try
        {
            IdentityResult result = await userManager.CreateAsync(identityUser, registerStudentDto.Password);
            
            if(!result.Succeeded)
            {
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })
                ));
            }

            if(registerStudentDto.UserType == UserType.Student)
            {
                Student student = registerStudentDto.ToStudent();
                student.IdentityId = identityUser.Id;

                applicationDbContext.Students.Add(student);
                await applicationDbContext.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return Ok(identityUser.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginStudentDto loginStudentDto)
    {
        IdentityUser? identityUser = await userManager.FindByEmailAsync(loginStudentDto.Email);

        if (identityUser is null || !await userManager.CheckPasswordAsync(identityUser, loginStudentDto.Password))
        {
            return Unauthorized();
        }

        return Ok("Logged in! Your token will be provided soon");
    }
}
