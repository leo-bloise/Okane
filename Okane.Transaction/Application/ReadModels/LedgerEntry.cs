namespace Okane.Transaction.Application.ReadModels;

public sealed record LedgerEntry(
    Guid Id,
    WalletSummary FromWallet,
    WalletSummary ToWallet,
    Guid OwnerId,
    decimal Amount,
    string? Description,
    DateTimeOffset RecordedAt,
    DateTimeOffset CreatedAt);
