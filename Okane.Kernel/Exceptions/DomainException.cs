using System.Net;

namespace Okane.Kernel.Exceptions;

public abstract class DomainException : Exception
{
    public HttpStatusCode StatusCode { get; private set; }

    public DomainException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }
}
