using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okane.Api.Contracts;
using Okane.Api.Contracts.Ledger.Responses;
using Okane.Transaction.Application.Interfaces;

namespace Okane.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/ledger")]
public sealed class LedgerController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken, int page = 1, int pageSize = 20)
    {
        var ownerId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var ledgerPage = await transactionService.GetLedgerForOwnerAsync(ownerId, page, pageSize, cancellationToken);

        var items = ledgerPage.Entries
            .Select(entry => new LedgerEntryResponse(
                entry.Id,
                new WalletSummaryResponse(entry.FromWallet.Id, entry.FromWallet.Name),
                new WalletSummaryResponse(entry.ToWallet.Id, entry.ToWallet.Name),
                entry.Amount,
                entry.Description,
                entry.RecordedAt))
            .ToList();

        var pageResponse = new LedgerPageResponse(items, ledgerPage.Page, ledgerPage.PageSize, ledgerPage.TotalCount);
        var response = ApiResponseFactory.Success(pageResponse, "Ledger retrieved successfully.");
        return StatusCode(response.Status, response);
    }
}
