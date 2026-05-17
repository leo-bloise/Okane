using Okane.Authentication.Requests;

namespace Okane.Infrastructure.Requests;

public class LoginUserRequest : ILoginUserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
