using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okane.Api.Contracts;
using Okane.Api.Contracts.Transactions.Requests;
using Okane.Api.Contracts.Transactions.Responses;
using Okane.Transaction.Application.Interfaces;

namespace Okane.Api.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public sealed class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            var transaction = await transactionService.RecordTransactionAsync(
                ownerId,
                request.FromWalletId,
                request.ToWalletId,
                request.Amount,
                request.Description,
                cancellationToken);

            var response = ApiResponseFactory.Success(
                new TransactionResponse(
                    transaction.Id,
                    transaction.FromWalletId,
                    transaction.ToWalletId,
                    transaction.Amount,
                    transaction.Description,
                    transaction.RecordedAt),
                "Transaction recorded successfully.",
                StatusCodes.Status201Created);

            return StatusCode(response.Status, response);
        }
        catch (ArgumentException ex)
        {
            var response = ApiResponseFactory.Error(ex.Message, StatusCodes.Status422UnprocessableEntity);
            return StatusCode(response.Status, response);
        }
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken, int page = 1, int pageSize = 20)
    {
        var ownerId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var pagedResult = await transactionService.GetLedgerForOwnerAsync(ownerId, page, pageSize, cancellationToken);

        var items = pagedResult.Entries
            .Select(entry => new TransactionResponse(
                entry.Id,
                entry.FromWallet.Id,
                entry.ToWallet.Id,
                entry.Amount,
                entry.Description,
                entry.RecordedAt))
            .ToList();

        var pageResponse = new TransactionsPageResponse(items, pagedResult.Page, pagedResult.PageSize, pagedResult.TotalCount);
        var response = ApiResponseFactory.Success(pageResponse, "Transactions retrieved successfully.");
        return StatusCode(response.Status, response);
    }
}
