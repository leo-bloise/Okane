using System.ComponentModel.DataAnnotations;

namespace Okane.Api.Contracts.Transactions.Requests;

public class CreateTransactionRequest
{
    [Required]
    public Guid FromWalletId { get; set; }

    [Required]
    public Guid ToWalletId { get; set; }

    [Range(0.01, 999999999)]
    public decimal Amount { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}
