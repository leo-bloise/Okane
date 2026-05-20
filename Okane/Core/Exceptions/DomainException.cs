namespace Okane.Core.Exceptions;

public class DomainException : Exception
{
    private readonly int _status;

    public int Status => _status;

    public DomainException(string message, int status) : base(message)
    {
        _status = status;
    }
}
