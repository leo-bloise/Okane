namespace Okane.Transaction.Application.Interfaces;

public sealed record WalletInfo(Guid Id, Guid OwnerId, bool IsActive);

/// <summary>
/// Port into the Wallet subdomain, used to validate that both sides of a
/// Transaction belong to the same owner and are Active before it is recorded.
/// </summary>
public interface IWalletLookup
{
    Task<WalletInfo?> GetWalletInfoAsync(Guid walletId, CancellationToken cancellationToken = default);
}
