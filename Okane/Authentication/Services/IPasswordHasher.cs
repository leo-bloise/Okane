namespace Okane.Authentication.Services;

public interface IPasswordHasher
{
    public Task<string> HashPasswordAsync(string plainPassword, CancellationToken cancellationToken);

    public Task<bool> VerifyPasswordAsync(string plainPassword, string hashedPassword, CancellationToken cancellationToken);
}
