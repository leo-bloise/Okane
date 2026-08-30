using System.Collections;

namespace Okane.Transaction.Application.ReadModels;

public sealed class Ledger(IEnumerable<LedgerEntry> entries) : IEnumerable<LedgerEntry>
{
    private readonly IReadOnlyList<LedgerEntry> _entries = entries.ToList();

    public IEnumerator<LedgerEntry> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
