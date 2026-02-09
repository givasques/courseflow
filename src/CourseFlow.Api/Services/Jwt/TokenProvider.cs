using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CourseFlow.Api.DTOs.Jwt;
using CourseFlow.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CourseFlow.Api.Services.Jwt;

public sealed class TokenProvider(IOptions<JwtAuthOptions> options, UserManager<IdentityUser> userManager)
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public async Task<AccessTokenDto> Create(IdentityUser identityUser)
    {
        return new AccessTokenDto(await GenerateAccessToken(identityUser), GenerateRefreshToken());
    }

    public async Task<string> GenerateAccessToken (IdentityUser identityUser)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roleList = await userManager.GetRolesAsync(identityUser);
        var role = roleList.SingleOrDefault() ?? throw new Exception("User has no role assigned.");

        List<Claim> claims = 
        [
            new (Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, identityUser.Id),
            new (Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, identityUser.Email!),
            new (ClaimTypes.Role, role)
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(claims),
          Expires = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes),
          SigningCredentials = credentials,
          Issuer = _jwtAuthOptions.Issuer,
          Audience = _jwtAuthOptions.Audience  
        };

        var handler = new JsonWebTokenHandler();

        string accessToken = handler.CreateToken(tokenDescriptor);

        return accessToken;
    }

    private string GenerateRefreshToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}
