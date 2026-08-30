namespace Okane.Api.Contracts.Wallets.Responses;

public sealed record WalletsPageResponse(
    IReadOnlyCollection<WalletResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
