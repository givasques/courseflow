using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CourseFlow.Api;

public sealed class TokenProvider(IOptions<JwtAuthOptions> options)
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public AccessTokenDto Create(TokenRequest request)
    {
        return new AccessTokenDto(GenerateAccessToken(request), GenerateRefreshToken());
    }

    public string GenerateAccessToken (TokenRequest request)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = 
        [
            new (Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, request.IdentityUserId),
            new (Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, request.IdentityUserEmail)
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
