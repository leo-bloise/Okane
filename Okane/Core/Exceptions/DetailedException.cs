using System.Collections.ObjectModel;

namespace Okane.Core.Exceptions;

public class DetailedException : DomainException
{
    private readonly IReadOnlyDictionary<string, object?> _details;

    public IReadOnlyDictionary<string, object?> Details => _details;

    public DetailedException(string message, int status, IDictionary<string, object?> details) : base(message, status) 
    {
        this._details = new ReadOnlyDictionary<string, object?>(details);
    }
}
