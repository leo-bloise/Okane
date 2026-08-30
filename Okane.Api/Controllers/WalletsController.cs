using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okane.Api.Contracts;
using Okane.Api.Contracts.Wallets.Requests;
using Okane.Api.Contracts.Wallets.Responses;
using Okane.Wallet.Application.Interfaces;

namespace Okane.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/wallets")]
public sealed class WalletsController(IWalletService walletService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateWalletRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            var wallet = await walletService.CreateStandardWalletAsync(ownerId, request.Name, cancellationToken);

            var response = ApiResponseFactory.Success(
                new WalletResponse(wallet.Id, wallet.Name, wallet.Kind, wallet.Status, wallet.CreatedAt),
                "Wallet created successfully.",
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
        var walletsPage = await walletService.GetWalletsForOwnerAsync(ownerId, page, pageSize, cancellationToken);

        var items = walletsPage.Items
            .Select(wallet => new WalletResponse(wallet.Id, wallet.Name, wallet.Kind, wallet.Status, wallet.CreatedAt))
            .ToList();

        var pageResponse = new WalletsPageResponse(items, walletsPage.Page, walletsPage.PageSize, walletsPage.TotalCount);
        var response = ApiResponseFactory.Success(pageResponse, "Wallets retrieved successfully.");
        return StatusCode(response.Status, response);
    }
}
