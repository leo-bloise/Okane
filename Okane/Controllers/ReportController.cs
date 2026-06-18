using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okane.Authentication;
using Okane.Infrastructure.Responses;
using Okane.Reports;
using Okane.Reports.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Okane.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportController(IDashboardReportGenerator dashboardReportGenerator): ControllerBase
{
    private User? GetUser()
    {
        Claim? subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (subClaim == null)
            return null;

        bool parsed = int.TryParse(subClaim.Value, out int userId);

        if (!parsed)
            return null;

        return new User()
        {
            Id = userId,
        };
    }

    [Authorize]
    [HttpGet("dashboardReport")]
    public async Task<IActionResult> DashboardReportAsync([FromQuery] int? month, CancellationToken cancellationToken)
    {
        User? user = GetUser();

        if (user is null) throw new UnauthorizedAccessException();

        if (!month.HasValue || month == 0) month = DateTime.Now.Month;

        DashboardReport report = await dashboardReportGenerator.GenerateAsync(month.Value, user.Id, cancellationToken);

        Dictionary<string, object?> data = new Dictionary<string, object?>()
        {
            {
                "balance", report.Balance
            },
            { 
                "incoming", report.TotalIncoming
            },
            {
                "liabilities", report.TotalLiabilities
            },
            {
                "balanceByDates", report.BalanceByDates
            }
        };

        return Ok(
            ResponsesFacade.Ok("Report generated successfully", data)
            );
    }
}
