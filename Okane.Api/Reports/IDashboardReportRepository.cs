namespace Okane.Api.Reports;

public interface IDashboardReportRepository
{
    Task<IEnumerable<DashboardReportRawDataItem>> GetRawInformationAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);
}
