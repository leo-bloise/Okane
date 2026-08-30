namespace Okane.Api.Reports;

public record DashboardReportRequest(
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    Guid UserId
);
