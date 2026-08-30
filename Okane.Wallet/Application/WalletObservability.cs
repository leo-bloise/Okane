using System.Diagnostics;

namespace Okane.Wallet.Application;

public static class WalletObservability
{
    public const string ActivitySourceName = "Okane.Wallet";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
