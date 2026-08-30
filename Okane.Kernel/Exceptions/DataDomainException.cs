using System.Net;

namespace Okane.Kernel.Exceptions;

public sealed class DataDomainException : DomainException
{
    public Dictionary<string, object>? Details { get; private set; }

    public DataDomainException(string message, HttpStatusCode statusCode, string? userMessage = null, Dictionary<string, object>? details = null) : base(message, statusCode, userMessage)
    {
        this.Details = details;
    }
}
