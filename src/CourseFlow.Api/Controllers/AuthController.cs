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
    IOptions<JwtAuthOptions> options,
    IEnumerable<IUserRegistrationStrategy> registrationStrategies) : ControllerBase
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        IdentityUser identityUser = registerDto.ToIdentityUser();

        try
        {
            IdentityResult result = await userManager.CreateAsync(identityUser, registerDto.Password);
            
            if(!result.Succeeded)
            {
                return ValidationProblem(new ValidationProblemDetails(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })
                ));
            }

            var registrationStrategy = registrationStrategies.First(s => s.Role == registerDto.UserRole.ToString());

            var strategyResult = await registrationStrategy.RegisterAsync(registerDto, identityUser);

            if(!strategyResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return ValidationProblem(new ValidationProblemDetails(
                    strategyResult.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })
                ));
            }
            
            AccessTokenDto tokens = await tokenProvider.Create(identityUser);

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
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        IdentityUser? identityUser = await userManager.FindByEmailAsync(loginDto.Email);

        if (identityUser is null || !await userManager.CheckPasswordAsync(identityUser, loginDto.Password))
        {
            return Unauthorized();
        }

        AccessTokenDto tokens = await tokenProvider.Create(identityUser);

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

        AccessTokenDto tokens = await tokenProvider.Create(refreshToken.User);

        refreshToken.Token = tokens.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationInDays);

        await identityDbContext.SaveChangesAsync();

        return Ok(tokens);
    }
}
