namespace Okane.Reports;

public record BalanceByDate(long Timepoint, decimal Value, decimal AccumulatedBalance);

public class DashboardReport
{
    public decimal Balance { get; private set; }

    public decimal TotalIncoming { get; private set; }

    public decimal TotalLiabilities { get; private set; }

    public IReadOnlyCollection<BalanceByDate> BalanceByDates { get; private set; }

    public DashboardReport(decimal balance, decimal totalIncoming, decimal totalLiabilities, IReadOnlyCollection<BalanceByDate> balanceByDates) 
    { 
        Balance = balance;
        TotalIncoming = totalIncoming;
        TotalLiabilities = totalLiabilities;
        BalanceByDates = balanceByDates;
    }
}
