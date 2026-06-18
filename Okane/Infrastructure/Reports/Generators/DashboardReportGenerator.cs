using Microsoft.EntityFrameworkCore;
using Okane.Ledger;
using Okane.Reports;
using Okane.Reports.Generators;

namespace Okane.Infrastructure.Reports.Generators;

public class DashboardReportGenerator(OkaneDbContext okaneDbContext): IDashboardReportGenerator
{
    private readonly string _externalAccountName = "External Account";

    public async Task<DashboardReport> GenerateAsync(int month, int userId, CancellationToken cancellationToken)
    {
        List<Transaction> transactions = await okaneDbContext.Ledger.Include(t => t.FromAccount)
            .Include(t => t.ToAccount)
            .Where(t => t.User.Id == userId && t.OccuredAt.Month == month)
            .ToListAsync(cancellationToken);        

        decimal liabilitiesAmount = GetLiabilitiesAmount(transactions);
        decimal incomingAmount = GetIncomingAmount(transactions);
        decimal balance = incomingAmount - liabilitiesAmount;

        IEnumerable<BalanceByDate> balanceByDates = CreateBalances(transactions);

        return new DashboardReport(balance, incomingAmount, liabilitiesAmount, [.. balanceByDates]);
    }

    private decimal GetIncomingAmount(List<Transaction> transactions)
    {
        return transactions
            .Where(t => t.FromAccount.Description.Equals(this._externalAccountName) && !t.ToAccount.Description.Equals(this._externalAccountName))
            .Sum(t => t.Amount);
    }

    private decimal GetLiabilitiesAmount(List<Transaction> transactions)
    {
        return transactions
            .Where(t => t.ToAccount.Description == this._externalAccountName)
            .Sum(t => t.Amount);
    }

    private IEnumerable<BalanceByDate> CreateBalances(List<Transaction> transactions)
    {
        List<BalanceByDate> balances = new List<BalanceByDate>();

        decimal acc = 0;

        foreach (var group in transactions.GroupBy(t => t.OccuredAt))
        {
            long timepoint = new DateTimeOffset(group.Key).ToUnixTimeMilliseconds();

            IEnumerable<Transaction> groupTransactions = [.. group];

            decimal incoming = GetIncomingAmount(groupTransactions.ToList());
            decimal liabilities = GetLiabilitiesAmount(groupTransactions.ToList());
            decimal balance = incoming - liabilities;

            acc += balance;

            balances.Add(new BalanceByDate(timepoint, balance, acc));
        }

        return balances;
    }
}
