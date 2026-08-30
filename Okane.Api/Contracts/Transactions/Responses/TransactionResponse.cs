namespace Okane.Api.Contracts.Transactions.Responses;

public sealed record TransactionResponse(
    Guid Id,
    Guid FromWalletId,
    Guid ToWalletId,
    decimal Amount,
    string? Description,
    DateTimeOffset RecordedAt);
