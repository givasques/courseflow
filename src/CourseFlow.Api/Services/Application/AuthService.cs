using CourseFlow.Api.Data;
using CourseFlow.Api.DTOs.Auth;
using CourseFlow.Api.DTOs.Jwt;
using CourseFlow.Api.Services.Jwt;
using CourseFlow.Api.Settings;
using CourseFlow.Api.Users.Registration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace CourseFlow.Api.Services.Application;

public sealed class AuthService(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext identityDbContext, 
    UserManager<IdentityUser> userManager,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options,
    IEnumerable<IUserRegistrationStrategy> registrationStrategies)
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public async Task<ServiceResult<AccessTokenDto>> Register(RegisterDto registerDto)
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
                await transaction.RollbackAsync();
                return ServiceResult<AccessTokenDto>.BadRequest<AccessTokenDto>(result.Errors);
            }

            var registrationStrategy = registrationStrategies.FirstOrDefault(s => s.Role == registerDto.UserRole.ToString());

            if (registrationStrategy is null)
            {
                await transaction.RollbackAsync();
                return ServiceResult<AccessTokenDto>.BadRequest<AccessTokenDto>("InvalidRole", "Invalid user role.");
            }

            var strategyResult = await registrationStrategy.RegisterAsync(registerDto, identityUser);

            if(!strategyResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return ServiceResult<AccessTokenDto>.BadRequest<AccessTokenDto>(strategyResult.Errors);
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
            return ServiceResult<AccessTokenDto>.Ok(tokens);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ServiceResult<AccessTokenDto>> Login(LoginDto loginDto)
    {
        IdentityUser? identityUser = await userManager.FindByEmailAsync(loginDto.Email);

        if (identityUser is null || !await userManager.CheckPasswordAsync(identityUser, loginDto.Password))
        {
            return ServiceResult<AccessTokenDto>.Unauthorized<AccessTokenDto>(
                "InvalidCredentials", 
                "Email or password is incorrect.");
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

        return ServiceResult<AccessTokenDto>.Ok(tokens);
    }

    public async Task<ServiceResult<AccessTokenDto>> Refresh(RefreshTokenDto refreshTokenDto)
    {
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

        if(refreshToken is null)
        {
            return ServiceResult<AccessTokenDto>.Unauthorized<AccessTokenDto>(
                "InvalidRefreshToken", 
                "The given refresh token is invalid.");
        }

        if (refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return ServiceResult<AccessTokenDto>.Unauthorized<AccessTokenDto>(
                "RefreshTokenExpired", 
                "The given refresh token is expired.");
        }

        AccessTokenDto tokens = await tokenProvider.Create(refreshToken.User);

        refreshToken.Token = tokens.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationInDays);

        await identityDbContext.SaveChangesAsync();

        return ServiceResult<AccessTokenDto>.Ok(tokens);
    }
}
