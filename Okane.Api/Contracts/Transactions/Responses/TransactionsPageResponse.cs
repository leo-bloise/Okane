namespace Okane.Api.Contracts.Transactions.Responses;

public sealed record TransactionsPageResponse(
    IReadOnlyCollection<TransactionResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
