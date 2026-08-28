namespace Okane.Wallet.Application.Interfaces;

/// <summary>
/// Port into the Transaction subdomain, used to enforce the rule that a Wallet
/// with recorded Transactions can only be archived, never deleted.
/// </summary>
public interface IWalletActivityChecker
{
    Task<bool> HasTransactionsAsync(Guid walletId, CancellationToken cancellationToken = default);
}
