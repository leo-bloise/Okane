using Microsoft.AspNetCore.Diagnostics;
using Okane.Api.Contracts;
using Okane.Kernel.Exceptions;

namespace Okane.Api.Infrastructure.ErrorHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ApiResponse response = exception switch
        {
            DataDomainException dataDomainException => HandleDomainException(dataDomainException, dataDomainException.Details),
            DomainException domainException => HandleDomainException(domainException, null),
            _ => HandleUnexpectedException(exception),
        };

        httpContext.Response.StatusCode = response.Status;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private ApiResponse HandleDomainException(DomainException exception, Dictionary<string, object>? details)
    {
        logger.LogWarning(exception, "Domain exception occurred: {Message}", exception.Message);

        var status = (int)exception.StatusCode;
        return details is null
            ? ApiResponseFactory.Error(exception.UserMessage, status)
            : ApiResponseFactory.Error(exception.UserMessage, status, details);
    }

    private ApiResponse HandleUnexpectedException(Exception exception)
    {
        logger.LogError(exception, "Unhandled exception occurred.");

        var details = environment.IsDevelopment() ? new { exception.Message } : null;
        return ApiResponseFactory.Error("An unexpected error occurred.", StatusCodes.Status500InternalServerError, details);
    }
}
