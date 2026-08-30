using System.Net;

namespace Okane.Kernel.Exceptions;

public abstract class DomainException : Exception
{
    public HttpStatusCode StatusCode { get; private set; }

    public string UserMessage { get; private set;  }

    public DomainException(string message, HttpStatusCode statusCode, string? userMessage = null) : base(message)
    {
        StatusCode = statusCode;
        this.UserMessage = userMessage ?? message;
    }
}
