using Okane.Authentication.Services;

namespace Okane.Infrastructure.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public Task<string> HashPasswordAsync(string plainPassword, CancellationToken cancellationToken)
    {
        return Task.FromResult(BCrypt.Net.BCrypt.HashPassword(plainPassword));
    }

    public Task<bool> VerifyPasswordAsync(string plainPassword, string hashedPassword, CancellationToken cancellationToken)
    {
        return Task.FromResult(BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword));
    }
}
