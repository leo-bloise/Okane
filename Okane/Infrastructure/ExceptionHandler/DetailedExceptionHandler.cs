using Microsoft.AspNetCore.Diagnostics;
using Okane.Core.Exceptions;
using Okane.Infrastructure.Responses;

namespace Okane.Infrastructure.ExceptionHandler;

public class DetailedExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        DetailedException? detailedException = exception as DetailedException;

        if (detailedException is null)
        {
            return false;
        }

        httpContext.Response.StatusCode = detailedException.Status;
        httpContext.Response.ContentType = "application/json";

        DetailedResponse response = ResponsesFacade.UnprocessableEntity(detailedException.Message, detailedException.Details);

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
