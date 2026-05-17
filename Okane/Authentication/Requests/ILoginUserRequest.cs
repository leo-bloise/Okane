using System.ComponentModel.DataAnnotations;

namespace Okane.Authentication.Requests;

public interface ILoginUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
