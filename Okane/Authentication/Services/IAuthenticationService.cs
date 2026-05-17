using Okane.Authentication.Requests;

namespace Okane.Authentication.Services;

public interface IAuthenticationService
{
    public Task<User> RegisterUserAsync(IRegisterUserRequest request, CancellationToken cancellationToken);

    public Task<string> LoginAsync(ILoginUserRequest request, CancellationToken cancellationToken);
}
