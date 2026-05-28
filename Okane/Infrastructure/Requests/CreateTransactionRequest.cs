using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Okane.Infrastructure.Attributes;
using Okane.Ledger.Requests;
using System.ComponentModel.DataAnnotations;

namespace Okane.Infrastructure.Requests;

public class CreateTransactionRequest : ICreateTransactionRequest
{
    [Required]
    [NotZero(ErrorMessage = "amount cannot be 0")]
    public decimal Amount { get; set; }
    
    [MinLength(10)]
    public string? Description { get; set; }
    
    [BindNever]
    [ValidateNever]
    public int UserId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int FromAccountId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int ToAccountId { get; set; }

    [Required]
    [RequiredDate(ErrorMessage = "occuredAt is required")]
    public DateTime OccuredAt { get; set; }
}
