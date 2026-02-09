using CourseFlow.Api.Controllers.Extensions;
using CourseFlow.Api.DTOs.Auth;
using CourseFlow.Api.DTOs.Jwt;
using CourseFlow.Api.Services.Application;
using Microsoft.AspNetCore.Mvc;

namespace CourseFlow.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var serviceResult = await authService.Register(registerDto);
        return this.FromServiceResult(serviceResult);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var serviceResult = await authService.Login(loginDto);
        return this.FromServiceResult(serviceResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto refreshTokenDto)
    {
        var serviceResult = await authService.Refresh(refreshTokenDto);
        return this.FromServiceResult(serviceResult);
    }
}
