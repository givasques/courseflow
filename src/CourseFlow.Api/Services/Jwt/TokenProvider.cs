using Microsoft.Extensions.Options;

namespace CourseFlow.Api;

public sealed class TokenProvider(IOptions<JwtAuthOptions> options)
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public AccessTokenDto Create(TokenRequest request)
    {
        return new AccessTokenDto();
    }

    public string GenerateAccessToken (TokenRequest request)
    {
        return string.Empty;
    }

    private string GenerateRefreshToken()
    {
        return string.Empty;
    }
}
