using System.ComponentModel.DataAnnotations;

namespace Okane.Api.Contracts.Wallets.Requests;

public class CreateWalletRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; }
}
