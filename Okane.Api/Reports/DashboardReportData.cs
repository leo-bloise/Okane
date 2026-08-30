namespace Okane.Api.Reports;

public record DashboardReportData(
    DateTimeOffset CreatedAt,
    decimal Balance,
    IEnumerable<DashboardReportWalletData> Wallets,
    decimal InFlow,
    decimal OutFlow
);
