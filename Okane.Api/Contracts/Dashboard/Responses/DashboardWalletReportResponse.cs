namespace Okane.Api.Contracts.Dashboard.Responses;

public sealed record DashboardWalletReportResponse(
    string Name,
    decimal InFlow,
    decimal OutFlow,
    decimal Balance);
