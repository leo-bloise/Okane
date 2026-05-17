using Okane.Authentication.Requests;
using System.ComponentModel.DataAnnotations;

namespace Okane.Infrastructure.Requests;

public record RegisterUserRequest() : IRegisterUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}
