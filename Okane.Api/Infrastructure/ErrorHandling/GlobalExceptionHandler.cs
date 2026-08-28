using Microsoft.AspNetCore.Diagnostics;
using Okane.Api.Contracts;

namespace Okane.Api.Infrastructure.ErrorHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred.");

        var details = environment.IsDevelopment() ? new { exception.Message } : null;
        var response = ApiResponseFactory.Error("An unexpected error occurred.", StatusCodes.Status500InternalServerError, details);

        httpContext.Response.StatusCode = response.Status;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
