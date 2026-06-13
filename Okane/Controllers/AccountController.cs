using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okane.Accounts;
using Okane.Accounts.Services;
using Okane.Authentication;
using Okane.Core.Data;
using Okane.Infrastructure.Requests;
using Okane.Infrastructure.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Okane.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AccountController(IAccountService service): ControllerBase
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

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] PageRequest request, CancellationToken cancellationToken)
    {
        User? user = GetUser();

        if (user is null) throw new UnauthorizedAccessException();

        Page<Account> page = await service.GetAccountPageAsync(request, user.Id, cancellationToken);

        Dictionary<string, object?> response = new()
        {
            { "items", page.Items.Select(a => new { id = a.Id, name = a.Name, description = a.Description }) },
            { "totalPages", page.TotalPages },
            { "pageSize", page.PageSize },
            { "pageIndex", page.PageIndex }
        };

        return Ok(ResponsesFacade.Ok("Accounts retrieved successfully", response));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountAsync(int id, CancellationToken cancellationToken)
    {
        User? user = GetUser();

        if (user is null) throw new UnauthorizedAccessException();

        Account? account = await service.GetAccountAsync(id, user.Id, cancellationToken);

        if (account is null)
            return NotFound(ResponsesFacade.NotFound("Account not found"));

        Dictionary<string, object?> response = new Dictionary<string, object?>()
        {
            { "id", account?.Id },
            { "name", account?.Name },
            { "description", account?.Description }
        };

        return Ok(ResponsesFacade.Ok("Account retrieved successfully", response));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAccount([FromQuery] string query, CancellationToken cancellationToken)
    {
        User? user = GetUser();

        if(user is null) throw new UnauthorizedAccessException();

        IEnumerable<Account> accounts = await service.GetAccountsAsync(query, cancellationToken);

        Dictionary<string, object?> response = new Dictionary<string, object?>()
        {
            {"results", accounts.Select(x =>
                {
                    return new {
                        id = x.Id,
                        name = x.Name,
                        description = x.Description
                    };
                }) 
            }
        };

        return Ok(ResponsesFacade.Ok("Search result", response));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccountAsync([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        User? user = GetUser();

        if (user is null) throw new UnauthorizedAccessException();

        request.User = user;

        Account account = await service.CreateAccountAsync(request, cancellationToken);

        Dictionary<string, object?> response = new()
        {
            { "id", account.Id },
            { "name", account.Name },
            { "description", account.Description }
        };

        return Created($"/account/{account.Id}", ResponsesFacade.Created("Account created successfully", response));
    }
}
