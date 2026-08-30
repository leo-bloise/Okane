namespace Okane.Api.Contracts.Ledger.Responses;

public sealed record LedgerPageResponse(
    IReadOnlyCollection<LedgerEntryResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
