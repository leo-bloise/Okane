using System.Diagnostics;

namespace Okane.Transaction.Application;

public static class TransactionObservability
{
    public const string ActivitySourceName = "Okane.Transaction";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
