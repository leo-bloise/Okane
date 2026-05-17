namespace Okane.Authentication.Repositories;

public interface IUserRepository
{
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

    public Task<User> SaveUserAsync(User user, CancellationToken cancellationToken);

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
}
