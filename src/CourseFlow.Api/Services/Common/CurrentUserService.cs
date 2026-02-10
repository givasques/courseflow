using System.Security.Claims;

namespace CourseFlow.Api.Services.Common;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? IdentityId => 
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated => 
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
