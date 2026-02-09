using Microsoft.AspNetCore.Mvc;

namespace CourseFlow.Api;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var serviceResult = await authService.Register(registerDto);
        if (!serviceResult.Success)
        {
            return StatusCode(serviceResult.StatusCode, new ValidationProblemDetails(serviceResult.Errors ?? []));
        }
        return Ok(serviceResult.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var serviceResult = await authService.Login(loginDto);
        if (!serviceResult.Success)
        {
            return StatusCode(serviceResult.StatusCode, new ValidationProblemDetails(serviceResult.Errors ?? []));
        }
        return Ok(serviceResult.Data);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto refreshTokenDto)
    {
        var serviceResult = await authService.Refresh(refreshTokenDto);
        if (!serviceResult.Success)
        {
            return StatusCode(serviceResult.StatusCode, new ValidationProblemDetails(serviceResult.Errors ?? []));
        }
        return Ok(serviceResult.Data);
    }
}
