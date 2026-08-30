using System.Collections;

namespace Okane.Api.Reports;


public class DashboardReportRawData : IEnumerable<DashboardReportRawDataItem>
{
    private readonly IEnumerable<DashboardReportRawDataItem> _items;

    public DashboardReportRawData(IEnumerable<DashboardReportRawDataItem> data)
    {
        _items = data;
    }

    public IEnumerator<DashboardReportRawDataItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class DashboardReportService(IDashboardReportRepository dashboardReportRepository)
{
    private DashboardReportData Consolidate(IEnumerable<DashboardReportRawDataItem> items)
    {
        Dictionary<Guid, DashboardReportWalletData> walletData = items
            .GroupBy(i => i.WalletId)
            .ToDictionary(g => g.Key, g => new DashboardReportWalletData(
                g.First().WalletName,
                g.Where(i => i.ToWalletId.Equals(g.Key)).Sum(i => i.Amount),
                g.Where(i => i.FromWalletId.Equals(g.Key)).Sum(i => i.Amount)
            ));

        return new DashboardReportData(
            DateTimeOffset.UtcNow,
            walletData.Values.Aggregate(0m, (acc, c) => acc + c.Balance),
            walletData.Values,
            walletData.Values.Aggregate(0m, (acc, c) => acc + c.InFlow),
            walletData.Values.Aggregate(0m, (acc, c) => acc + c.OutFlow)
        );
    }

    public async Task<DashboardReportData> GenerateReportAsync(DashboardReportRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<DashboardReportRawDataItem> items = await dashboardReportRepository.GetRawInformationAsync(
            request.UserId,
            request.StartTime,
            request.EndTime,
            cancellationToken
        );

        return Consolidate(items);
    }
}
