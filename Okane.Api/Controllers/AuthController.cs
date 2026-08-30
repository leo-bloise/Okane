using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Okane.Api.Contracts;
using Okane.Api.Contracts.Auth.Requests;
using Okane.Api.Infrastructure.Security;
using Okane.User.Application.Interfaces;

namespace Okane.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IUserService userService,
    JwtTokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userService.CreateUserAsync(request.Name, request.Email, request.Password, cancellationToken);
            var response = ApiResponseFactory.Success(
                new { user.Id, user.Name, user.Email },
                "User registered successfully.",
                StatusCodes.Status201Created);

            return StatusCode(response.Status, response);
        }
        catch (InvalidOperationException)
        {
            var response = ApiResponseFactory.Error("These credentials are not allowed to be used. Try again later", StatusCodes.Status422UnprocessableEntity);
            return StatusCode(response.Status, response);
        }
        catch (ArgumentException ex)
        {
            var response = ApiResponseFactory.Error(ex.Message, StatusCodes.Status422UnprocessableEntity);
            return StatusCode(response.Status, response);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);
        if (user is null)
        {
            var errorResponse = ApiResponseFactory.Error("Invalid email or password.", StatusCodes.Status401Unauthorized);
            return StatusCode(errorResponse.Status, errorResponse);
        }

        var token = tokenService.GenerateToken(user);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(jwtOptions.Value.ExpiryMinutes);

        var isDevelopment = environment.IsDevelopment();
        Response.Cookies.Append(AuthCookieNames.AccessToken, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment,
            SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = expiresAt,
            Path = "/"
        });

        var response = ApiResponseFactory.Success(new { expiresAt }, "Login successful.");
        return StatusCode(response.Status, response);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var id = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var name = User.FindFirstValue("name");

        var response = ApiResponseFactory.Success(new { id, name, email }, "User retrieved successfully.");
        return StatusCode(response.Status, response);
    }
}
