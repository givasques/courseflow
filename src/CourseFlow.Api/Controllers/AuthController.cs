using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace CourseFlow.Api;

[ApiController]
[Route("auth")]
public class AuthController(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext identityDbContext, 
    UserManager<IdentityUser> userManager,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options) : ControllerBase
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

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

            var roleResult = await userManager.AddToRoleAsync(identityUser, Roles.Student);

            if(!roleResult.Succeeded)
            {
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })
                ));
            }
            
            Student student = registerStudentDto.ToStudent();
            student.IdentityId = identityUser.Id;

            applicationDbContext.Students.Add(student);
            await applicationDbContext.SaveChangesAsync();
            
            var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!);
            AccessTokenDto tokens = tokenProvider.Create(tokenRequest);

            var refreshToken = new RefreshToken()
            {
              Id = Guid.NewGuid(),
              UserId = identityUser.Id,
              Token = tokens.RefreshToken,
              ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationInDays)  
            };

            identityDbContext.RefreshTokens.Add(refreshToken);
            await identityDbContext.SaveChangesAsync();

            await transaction.CommitAsync();
            return Ok(tokens);
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

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!);
        AccessTokenDto tokens = tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = identityUser.Id,
            Token = tokens.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationInDays)  
        };

        identityDbContext.RefreshTokens.Add(refreshToken);
        await identityDbContext.SaveChangesAsync();

        return Ok(tokens);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto refreshTokenDto)
    {
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

        if(refreshToken is null)
        {
            return Unauthorized();
        }

        if (refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Unauthorized();
        }

        var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!);
        AccessTokenDto tokens = tokenProvider.Create(tokenRequest);

        refreshToken.Token = tokens.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationInDays);

        await identityDbContext.SaveChangesAsync();

        return Ok(tokens);
    }
}
