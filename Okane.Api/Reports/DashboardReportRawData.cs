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
