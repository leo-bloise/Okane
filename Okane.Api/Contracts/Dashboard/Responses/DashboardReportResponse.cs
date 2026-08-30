namespace Okane.Api.Contracts.Dashboard.Responses;

public sealed record DashboardReportResponse(
    DateTimeOffset CreatedAt,
    decimal Balance,
    decimal InFlow,
    decimal OutFlow,
    IReadOnlyCollection<DashboardWalletReportResponse> Wallets);
