namespace Okane.Reports.Generators;

public interface IDashboardReportGenerator
{
    public Task<DashboardReport> GenerateAsync(int month, int userId, CancellationToken cancellationToken);
}
