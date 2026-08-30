namespace Okane.Api.Contracts.Ledger.Responses;

public sealed record LedgerEntryResponse(
    Guid Id,
    WalletSummaryResponse FromWallet,
    WalletSummaryResponse ToWallet,
    decimal Amount,
    string? Description,
    DateTimeOffset RecordedAt);
