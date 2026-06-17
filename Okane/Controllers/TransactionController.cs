using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okane.Accounts;
using Okane.Authentication;
using Okane.Core.Data;
using Okane.Infrastructure.Requests;
using Okane.Infrastructure.Responses;
using Okane.Ledger;
using Okane.Ledger.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Okane.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TransactionController(ILedgerService ledgerService) : ControllerBase
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

    private Dictionary<string, object?> MapTransaction(Transaction transaction)
    {
        return new Dictionary<string, object?>()
        {
            { "id", transaction.Id },
            { "amount", transaction.Amount },
            { "description", transaction.Description },
            { "fromAccountId", transaction.FromAccountId },
            { "toAccountId", transaction.ToAccountId },
            { "occuredAt", transaction.OccuredAt }
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] PageRequest request, CancellationToken cancellationToken)
    {
        User? user = GetUser();

        if (user is null) throw new UnauthorizedAccessException();

        Page<Transaction> page = await ledgerService.GetPagedTransaction(user.Id, request.Page, request.PageSize, cancellationToken);

        Dictionary<string, object?> response = new()
        {
            { "items", page.Items.Select(t => this.MapTransaction(t)) },
            { "totalPages", page.TotalPages },
            { "pageSize", page.PageSize },
            { "pageIndex", page.PageIndex }
        };

        return Ok(ResponsesFacade.Ok("Accounts retrieved successfully", response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionAsync(int id, CancellationToken cancellationToken)
    {
        User? user = GetUser() ?? throw new UnauthorizedAccessException();

        Transaction? transaction = await ledgerService.GetTransactionAsync(id, user.Id, cancellationToken);

        if(transaction is null)
        {
            return NotFound(ResponsesFacade.NotFound($"Transaction not found {id}"));
        }

        return Ok(ResponsesFacade.Ok($"Transaction {id}", MapTransaction(transaction)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransactionAsync([FromBody] CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        User? user = GetUser() ?? throw new UnauthorizedAccessException();

        request.UserId = user.Id;

        Transaction transaction = await ledgerService.CreateTransactionAsync(request, cancellationToken);

        return Created($"/transaction/{transaction.Id}", ResponsesFacade.Created("Transaction created", this.MapTransaction(transaction)));
    }
}
