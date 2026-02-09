namespace CourseFlow.Api.DTOs.Jwt;

public sealed record AccessTokenDto (string AccessToken, string RefreshToken);
