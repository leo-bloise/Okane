using System.ComponentModel.DataAnnotations;

namespace Okane.Api.Contracts.Auth.Requests;

public class RegisterRequest
{
    [StringLength(200, MinimumLength = 1)]
    [Required]
    public string Name { get; set; }

    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}
