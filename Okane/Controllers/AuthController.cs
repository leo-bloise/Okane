using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okane.Infrastructure.Requests;
using Okane.Infrastructure.Responses;

namespace Okane.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(Authentication.Services.IAuthenticationService authenticationService, ILogger<AuthController> logger): ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(User);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Register user requested");

        var user = await authenticationService.RegisterUserAsync(request, cancellationToken);

        Dictionary<string, object> details = new()
        {
            { "UserId", user.Id.ToString() },
            { "Email", user.Email }
        };

        return Created("/auth/me", ResponsesFacade.Created("User created successfully", details));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Login user requested");
        string token = await authenticationService.LoginAsync(request, cancellationToken);

        Dictionary<string, object> details = new()
        {
            { "token", token },
        };

        return Ok(ResponsesFacade.Ok("Login successful", details));
    }
}