using System.ComponentModel.DataAnnotations;

namespace Okane.Api.Contracts.Auth.Requests;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
