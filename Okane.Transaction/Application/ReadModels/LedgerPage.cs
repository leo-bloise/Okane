namespace Okane.Transaction.Application.ReadModels;

public sealed record LedgerPage(Ledger Entries, int Page, int PageSize, int TotalCount);
