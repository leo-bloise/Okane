namespace Okane.Api.Reports;

public record DashboardReportWalletData(
    string Name,
    decimal InFlow,
    decimal OutFlow
)
{
    public decimal Balance { get => InFlow - OutFlow; }
}
