using Microsoft.AspNetCore.Diagnostics;
using Okane.Core.Exceptions;
using Okane.Infrastructure.Responses;

namespace Okane.Infrastructure.ExceptionHandler;

public class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        DomainException? domainException = exception as DomainException;

        if (domainException is null) {
            return false;
        }

        httpContext.Response.StatusCode = domainException.Status;
        httpContext.Response.ContentType = "application/json";

        BaseResponse response = ResponsesFacade.CreateBaseResponse(domainException.Message, domainException.Status);

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
