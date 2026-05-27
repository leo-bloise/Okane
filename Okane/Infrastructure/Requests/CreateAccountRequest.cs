using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Okane.Accounts.Requests;
using Okane.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Okane.Infrastructure.Requests;

public class CreateAccountRequest : ICreateAccountRequest
{
    [Required]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    public string Name { get; set; }

    [Required]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters long.")]
    public string Description { get; set; }

    [ValidateNever]
    [BindNever]
    public User User { get; set; }
}
