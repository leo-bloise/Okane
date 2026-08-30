using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okane.Api.Contracts;
using Okane.Api.Contracts.Dashboard.Responses;
using Okane.Api.Reports;

namespace Okane.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController(DashboardReportService dashboardReportService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetReport(DateTimeOffset? startTime, DateTimeOffset? endTime, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var (currentMonthStart, currentMonthEnd) = GetCurrentMonthRange();
        var request = new DashboardReportRequest(startTime ?? currentMonthStart, endTime ?? currentMonthEnd, userId);

        DashboardReportData report = await dashboardReportService.GenerateReportAsync(request, cancellationToken);

        var walletResponses = report.Wallets
            .Select(wallet => new DashboardWalletReportResponse(wallet.Name, wallet.InFlow, wallet.OutFlow, wallet.Balance))
            .ToList();

        var response = ApiResponseFactory.Success(
            new DashboardReportResponse(report.CreatedAt, report.Balance, report.InFlow, report.OutFlow, walletResponses),
            "Dashboard report generated successfully.");

        return StatusCode(response.Status, response);
    }

    private static (DateTimeOffset Start, DateTimeOffset End) GetCurrentMonthRange()
    {
        var now = DateTimeOffset.UtcNow;
        var start = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddMonths(1).AddTicks(-1);

        return (start, end);
    }
}
