namespace CourseFlow.Api.Services.Common;

public interface ICurrentUserService
{
    string? IdentityId { get; }
    public bool IsAuthenticated { get; }
}
